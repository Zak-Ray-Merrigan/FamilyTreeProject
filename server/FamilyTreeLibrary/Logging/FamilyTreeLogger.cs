using FamilyTreeLibrary.Infrastructure.Resource;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary.Logging
{
    public class FamilyTreeLogger : IExtendedLogger
    {
        private readonly string categoryName;
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger? fallbackLogger;

        public FamilyTreeLogger(string categoryName, FamilyTreeVault vault, ILogger? fallbackLogger = null)
        {
            this.categoryName = categoryName;
            this.fallbackLogger = fallbackLogger;
            TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            string instrumentationKey = vault["ApplicationInsightsInstrumentationKey"].AsString;
            config.ConnectionString = $"InstrumentationKey={instrumentationKey}";
            telemetryClient = new(config);
            telemetryClient.Context.Cloud.RoleName = "FamilyTreeApplication";
            telemetryClient.Context.Component.Version = typeof(FamilyTreeLogger).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        }

        public FamilyTreeLogger(string categoryName, TelemetryClient telemetryClient, ILogger? fallbackLogger = null)
        {
            this.categoryName = categoryName;
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.fallbackLogger = fallbackLogger;
        }

        public TelemetryClient TelemetryClient => telemetryClient;

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return fallbackLogger?.BeginScope(state) ?? null;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return ((ILogger)this).BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            fallbackLogger?.Log(logLevel, eventId, state, exception, formatter);
            if (!IsEnabled(logLevel))
            {
                return;
            }
            string message = formatter(state, exception);
            SeverityLevel level = logLevel switch
            {
                LogLevel.Critical => SeverityLevel.Critical,
                LogLevel.Error => SeverityLevel.Error,
                LogLevel.Warning => SeverityLevel.Warning,
                LogLevel.Information => SeverityLevel.Information,
                LogLevel.Debug => SeverityLevel.Verbose,
                LogLevel.Trace => SeverityLevel.Verbose,
                _ => SeverityLevel.Information
            };
            IDictionary<string,string> properties = new Dictionary<string,string>()
            {
                ["CategoryName"] = categoryName,
                ["EventId"] = eventId.ToString()
            };
            if (state is IEnumerable<KeyValuePair<string,object>> stateProps)
            {
                foreach (KeyValuePair<string,object> prop in stateProps)
                {
                    if (prop.Value is not null && !properties.ContainsKey(prop.Key))
                    {
                        properties[prop.Key] = prop.Value.ToString()!;
                    }
                }
            }
            if (exception is not null)
            {
                ExceptionTelemetry exceptionTelemetry = new(exception)
                {
                    SeverityLevel = level,
                    Message = exception.Message
                };
                foreach (KeyValuePair<string,string> prop in properties)
                {
                    exceptionTelemetry.Properties.Add(prop.Key, prop.Value);
                }
                telemetryClient.TrackException(exceptionTelemetry);
            }
            else
            {
                telemetryClient.TrackTrace(message, level, properties);
            }
        }

        public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
        {
            telemetryClient.TrackEvent(eventName, properties, metrics);
        }

        public void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            telemetryClient.TrackDependency(dependencyType, dependencyName, data, startTime, duration, success);
        }
        public void TrackOperation(string operationName, Action operation, IDictionary<string, string>? properties = null)
        {
            using IOperationHolder<RequestTelemetry>? operation_ = telemetryClient.StartOperation<RequestTelemetry>(operationName);
            
            try
            {
                if (properties != null && operation_.Telemetry != null)
                {
                    foreach (var prop in properties)
                    {
                        operation_.Telemetry.Properties.Add(prop.Key, prop.Value);
                    }
                }
                
                operation();
                
                if (operation_.Telemetry != null)
                    operation_.Telemetry.Success = true;
            }
            catch (Exception ex)
            {
                if (operation_.Telemetry != null)
                    operation_.Telemetry.Success = false;
                
                Log(LogLevel.Error, new EventId(), $"Operation {operationName} failed", ex, (s, e) => s);
                throw;
            }
        }
    }

    public class FamilyTreeLogger<T> : FamilyTreeLogger, IExtendedLogger<T>
    {
        public FamilyTreeLogger(FamilyTreeVault vault, ILogger<T>? fallbackLogger = null)
            :base(typeof(T).FullName ?? nameof(T), vault, fallbackLogger) {}
        public FamilyTreeLogger(TelemetryClient telemetryClient, ILogger<T>? fallbackLogger = null)
            :base(typeof(T).FullName ?? nameof(T), telemetryClient, fallbackLogger){}
    }
}