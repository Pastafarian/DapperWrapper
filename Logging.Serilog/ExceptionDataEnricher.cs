using System.Collections;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Logging.Serilog
{
    public class ExceptionDataEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception?.Data == null || logEvent.Exception.Data.Count == 0) return;

            var dataDictionary = logEvent.Exception.Data
                .Cast<DictionaryEntry>()
                .Where(e => e.Key is string)
                .ToDictionary(e => (string)e.Key, e => e.Value);

            var property = propertyFactory.CreateProperty("ExceptionData", dataDictionary,  true);
            
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
