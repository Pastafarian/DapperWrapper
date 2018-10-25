using System;
using Serilog;
using Serilog.Events;

namespace Logging.Serilog
{
    public static class SerilogBuilder
    {
        private const string FileOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}] {Message:lj}{NewLine}{Exception}{Properties}{NewLine}{NewLine}";
        private const string EventOutputTemplate = "{Message}{NewLine}{Exception}{NewLine}{Properties}";

        public static global::Serilog.ILogger Build<TImplementingType>(LoggerSettings settings)
        {
            return new LoggerConfiguration()
                .Enrich.With<ExceptionDataEnricher>()
                .WriteTo.File(settings.LogFileLocation, outputTemplate: FileOutputTemplate, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, shared: true)
                .WriteTo.EventLog(settings.ApplicationName, outputTemplate: EventOutputTemplate)
                .MinimumLevel.Is(MapLogEventLevel(settings.LogEventLevel))
                .CreateLogger()
                .ForContext(typeof(TImplementingType));
        }

        public static global::Serilog.ILogger Build(LoggerSettings settings)
        {
            return new LoggerConfiguration()
                .Enrich.With<ExceptionDataEnricher>()
                .WriteTo.File(settings.LogFileLocation, outputTemplate: FileOutputTemplate, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, shared: true)
                .WriteTo.EventLog(settings.ApplicationName, outputTemplate: EventOutputTemplate)
                .MinimumLevel.Is(MapLogEventLevel(settings.LogEventLevel))
                .CreateLogger();
        }

        private static LogEventLevel MapLogEventLevel(LoggingEventType type)
        {
            switch (type)
            {
                case LoggingEventType.Verbose:
                    return LogEventLevel.Verbose;
                case LoggingEventType.Debug:
                    return LogEventLevel.Debug;
                case LoggingEventType.Information:
                    return LogEventLevel.Information;
                case LoggingEventType.Warning:
                    return LogEventLevel.Warning;
                case LoggingEventType.Error:
                    return LogEventLevel.Error;
                case LoggingEventType.Fatal:
                    return LogEventLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}