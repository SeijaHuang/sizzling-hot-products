using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Web.Host.Common;

namespace Web.Host.Helpers;

public class DateTimeConverter : JsonConverter<DateTime>
{

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        foreach (var format in DateFormatConstants.SupportedFormats)
        {
            if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var date))
            {
                return date;
            }
        }

        throw new JsonException($"Unable to parse '{value}' as DateTime.");

    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}

