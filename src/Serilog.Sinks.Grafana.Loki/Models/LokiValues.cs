using System.Text.Json.Serialization;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki.Infrastructure;

namespace Serilog.Sinks.Grafana.Loki.Models;

/// <summary>
/// Values sent to loki by entry
/// </summary>
[JsonConverter(typeof(LokiValuesConverter))]
public class LokiValues
{
    /// <summary>
    /// Create loki values
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="value"></param>
    /// <param name="metadata"></param>
    public LokiValues(DateTimeOffset timestamp, string value, IReadOnlyDictionary<string, LogEventPropertyValue> metadata)
    {
        Timestamp = timestamp;
        Value = value;
        Metadata = metadata;
    }

    /// <summary>
    /// Timestamp of the log event
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Text value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Metadata of the log event
    /// </summary>
    public IReadOnlyDictionary<string, LogEventPropertyValue> Metadata { get; set; } = new Dictionary<string, LogEventPropertyValue>();
}
