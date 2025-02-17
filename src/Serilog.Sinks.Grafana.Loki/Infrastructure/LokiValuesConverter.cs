using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki.Models;
using Serilog.Sinks.Grafana.Loki.Utils;

namespace Serilog.Sinks.Grafana.Loki.Infrastructure;

/// <summary>
/// Convert loki values to json
/// </summary>
public class LokiValuesConverter : JsonConverter<LokiValues>
{
    /// <summary>
    /// Not implemented because we don't need to read loki values
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override LokiValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Write loki values in expected format
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, LokiValues value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteStringValue(value.Timestamp.ToUnixNanosecondsString());
        writer.WriteStringValue(value.Value);

        if (value.Metadata.Count > 0)
        {
            writer.WriteStartObject();
            foreach (var item in value.Metadata)
            {
                WriteLogEventPropertyValue(item.Key, writer, item.Value, options);
            }

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }

    private void WriteLogEventPropertyValue(string key, Utf8JsonWriter writer, LogEventPropertyValue propertyValue, JsonSerializerOptions options)
    {
        if (propertyValue is ScalarValue sv)
        {
            writer.WritePropertyName(key);

            if (sv.Value is DateTime svdt)
            {
                writer.WriteStringValue(svdt.ToString("o"));
            }
            else if (sv.Value is DateTimeOffset svdto)
            {
                writer.WriteStringValue(svdto.ToString("o"));
            }
            else
            {
                // Loki wants string values
                writer.WriteStringValue(sv.Value?.ToString() ?? "<null>");
            }
        }

        if (propertyValue is SequenceValue seqv)
        {
            writer.WritePropertyName(key);

            // We write all data in sequence as string
            writer.WriteStringValue(seqv.ToString() ?? "<null>");
        }

        if (propertyValue is StructureValue strv)
        {
            foreach (var strvItem in strv.Properties)
            {
                var newKey = $"{key}_{strvItem.Name}";
                WriteLogEventPropertyValue(newKey, writer, strvItem.Value, options);
            }
        }

        if (propertyValue is DictionaryValue dictv)
        {
            foreach (var dictvItem in dictv.Elements)
            {
                var newKey = $"{key}_{dictvItem.Key.Value?.ToString()}";
                WriteLogEventPropertyValue(newKey, writer, dictvItem.Value, options);
            }
        }
    }
}
