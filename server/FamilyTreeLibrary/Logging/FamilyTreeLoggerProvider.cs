using FamilyTreeLibrary.Infrastructure.Resource;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary.Logging
{
    public class FamilyTreeLoggerProvider : ILoggerProvider
    {
        private readonly string instrumentationKey;
        private readonly TelemetryClient telemetryClient;
        private readonly ILoggerProvider? fallbackProvider;

        public FamilyTreeLoggerProvider(FamilyTreeVault vault, ILoggerProvider? fallbackProvider = null)
        {
            instrumentationKey = vault["ApplicationInsightsInstrumentationKey"].AsString;
            this.fallbackProvider = fallbackProvider;
            TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = $"InstrumentationKey={instrumentationKey}";
            telemetryClient = new(config);
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            ILogger? fallbackLogger = fallbackProvider?.CreateLogger(categoryName);
            return new FamilyTreeLogger(categoryName, telemetryClient, fallbackLogger);
        }

        public void Dispose()
        {
            telemetryClient.Flush();
            fallbackProvider?.Dispose();
            Task.Delay(3000).Wait();
        }
    }
}