namespace Orleans.Telemetry.SerilogConsumer
{
    using System;
    using System.Collections.Generic;
    using Runtime;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

    static class OrleansSerilogExtensions
    {
        public static ILogger ForContext<T>(this ILogger logger, IDictionary<string, T> properties, bool destructureObjects = false)
        {
            if (properties == null || properties.Count == 0)
            {
                return logger;
            }

            return logger.ForContext(new DictionaryEventEnricher<T>(properties, destructureObjects));
        }

        class DictionaryEventEnricher<T> : ILogEventEnricher
        {
            readonly IDictionary<string, T> _properties;
            readonly bool _destructureObjects;

            public DictionaryEventEnricher(IDictionary<string, T> properties, bool destructureObjects)
            {
                _properties = properties;
                _destructureObjects = destructureObjects;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                foreach (var kvp in _properties)
                {
                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(kvp.Key, kvp.Value, _destructureObjects));
                }
            }
        }

        public static LogEventLevel ToLogEventLevel(this Severity severity)
        {
            switch (severity)
            {
                case Severity.Error:
                    return LogEventLevel.Error;
                case Severity.Warning:
                    return LogEventLevel.Warning;
                case Severity.Info:
                    return LogEventLevel.Information;
                case Severity.Verbose:
                case Severity.Verbose2:
                case Severity.Verbose3:
                case Severity.Off:
                    return LogEventLevel.Verbose;

                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }
    }
}