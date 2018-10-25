using System;

namespace Logging.Serilog
{
    public class LoggerSettings
    {
        /// <summary>
        /// The location fo the log file to be written to on disk
        /// </summary>
        public string LogFileLocation { get; set; }

        /// <summary>
        /// Name of the application being run. This is used to write to the event logs on the server. 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Sets the threshold at which the application logs events. 
        /// </summary>
        public LoggingEventType LogEventLevel { get; set; }

        public LoggerSettings()
        {
        }

        public LoggerSettings(LoggingEventType logEventLevel, string applicationName, string logFileLocation)
        {
            LogEventLevel = logEventLevel;
            ApplicationName = applicationName;
            LogFileLocation = logFileLocation;
        }

        public LoggerSettings(string logEventLevel, string applicationName, string logFileLocation)
        {
            LogEventLevel = MapLogEventLevel(logEventLevel);
            ApplicationName = applicationName;
            LogFileLocation = logFileLocation;
        }


        private static LoggingEventType MapLogEventLevel(string logEventLevel)
        {
            logEventLevel = logEventLevel.ToLower();

            switch (logEventLevel)
            {
                case "verbose":
                    return LoggingEventType.Verbose;
                case "debug":
                    return LoggingEventType.Debug;
                case "information":
                    return LoggingEventType.Information;
                case "warning":
                    return LoggingEventType.Warning;
                case "error":
                    return LoggingEventType.Error;
                case "fatal":
                    return LoggingEventType.Fatal;
                default:
                    throw new Exception("Event level not recognised.");
            }
        }
    }
}