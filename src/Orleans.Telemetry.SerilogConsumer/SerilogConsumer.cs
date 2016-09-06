namespace Orleans.Telemetry.SerilogConsumer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Runtime;
    using Serilog;

    public class SerilogConsumer : ICloseableLogConsumer, IDependencyTelemetryConsumer, IEventTelemetryConsumer, IExceptionTelemetryConsumer, IMetricTelemetryConsumer, ITraceTelemetryConsumer
    {
        readonly ILogger _logger;
        ILogger Logger => _logger ?? Serilog.Log.Logger;

        public SerilogConsumer()
        {
        }

        public SerilogConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public void Flush()
        {
            // Serilog doesn't have a Flush, but will perform a flush on Dispose (see Close)
        }

        public void Close()
        {
            if (_logger == null)
            {
                Serilog.Log.CloseAndFlush();
            }
            else
            {
                // Serilog's ILogger doesn't natively implement IDisposable, but --annoyingly-- the standard concrete implementation does
                // this is the recommended way to deal with closing and flushing
                (_logger as IDisposable)?.Dispose();
            }
        }

        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null) => 
            Logger.ForContext(properties)
                .ForContext(metrics)
                .Error(exception, "TrackException");

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null) => 
            Logger.ForContext(properties)
                .ForContext(metrics)
                .Information("TrackEvent {EventName:l}", eventName);

        public void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success) =>
            Logger.Information("TrackDependency: {Dependency:l}:{Command:l} ({DependencySuccess}) started at {DependencyStartTime} and took {DependencyDuration}.", dependencyName, commandName, success ? "Success" : "Failure", startTime, duration);

        public void Log(Severity severity, LoggerType loggerType, string caller, string message, IPEndPoint myIPEndPoint, Exception exception, int eventCode = 0)
        {
            if (severity != Severity.Off)
            {
                var logLevel = severity.ToLogEventLevel();
                var logger =Logger.ForContext("OrleansLoggerType", loggerType)
                    .ForContext("EventCode", eventCode)
                    .ForContext("IPEndPoint", myIPEndPoint);

                if (exception != null)
                {
                    logger.Write(logLevel, exception, message);
                }
                else
                {
                    logger.Write(logLevel, message);
                }
            }
        }

        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null) => 
            Logger.ForContext(properties)
                .Information($"TrackMetric {{{name}:l}}", value);

        public void TrackMetric(string name, TimeSpan value, IDictionary<string, string> properties = null) =>
            Logger.ForContext(properties)
                .Information($"TrackMetric {{{name}:l}}", value);

        public void IncrementMetric(string name)
        {
            /* not implemented
             * Serilog is not meant to be a full APM suite
             */
        }

        public void IncrementMetric(string name, double value)
        {
            /* not implemented
             * Serilog is not meant to be a full APM suite
             */
        }

        public void DecrementMetric(string name)
        {
            /* not implemented
             * Serilog is not meant to be a full APM suite
             */
        }

        public void DecrementMetric(string name, double value)
        {
            /* not implemented
             * Serilog is not meant to be a full APM suite
             */
        }

        public void TrackTrace(string message) => Logger.Debug(message);

        public void TrackTrace(string message, Severity severity)
        {
            if (severity != Severity.Off)
            {
                var logLevel = severity.ToLogEventLevel();
                Logger.Write(logLevel, message);
            }
        }

        public void TrackTrace(string message, Severity severity, IDictionary<string, string> properties)
        {
            if (severity != Severity.Off)
            {
                var logLevel = severity.ToLogEventLevel();
                Logger.ForContext(properties)
                    .Write(logLevel, message);
            }
        }

        public void TrackTrace(string message, IDictionary<string, string> properties) =>
            Logger.ForContext(properties)
                .Debug(message);
    }
}
