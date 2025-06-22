using FamilyTreeLibrary.Data.Enumerators;
using FamilyTreeLibrary.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary
{
    public static partial class FamilyTreeUtils
    {
        public static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddUserSecrets<FamilyTreeDbSettings>()
                .Build();
        }

        public static void InitializeLogger()
        {
            string filePath = Path.Combine(GetRootDirectory(),@"resources\Logs\log.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void LogMessage(LoggingLevels level, string message)
        {
            switch (level)
            {
                case LoggingLevels.Debug: Log.Debug(message); break;
                case LoggingLevels.Information: Log.Information(message); break;
                case LoggingLevels.Warning: Log.Warning(message); break;
                case LoggingLevels.Error: Log.Error(message); break;
                case LoggingLevels.Fatal: Log.Fatal(message); break;
            }
        }

        [GeneratedRegex(@"^\d+$")]
        internal static partial Regex NumberPattern();

        [GeneratedRegex(@"^\d+-\d+$")]
        internal static partial Regex RangePattern();
        public static string GetRootDirectory()
        {
            Assembly assemblyLocation = Assembly.GetExecutingAssembly();
            FileInfo file = new(assemblyLocation.Location);
            DirectoryInfo currentDirectory = file.Directory;
            while (currentDirectory.Name != "FamilyTreeProject")
            {
                DirectoryInfo temp = currentDirectory;
                currentDirectory = temp.Parent;
            }
            return currentDirectory.FullName;
        }
    }
}