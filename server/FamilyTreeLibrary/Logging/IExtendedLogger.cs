using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary.Logging
{
    public interface IExtendedLogger : ILogger
    {
        public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null);

        public void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success);
        public void TrackOperation(string operationName, Action operation, IDictionary<string, string>? properties = null);
    }

    public interface IExtendedLogger<T> : IExtendedLogger {}
}