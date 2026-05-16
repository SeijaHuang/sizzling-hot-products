using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Host.Helpers;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    #region Private Fields
    private static readonly ImmutableArray<string> _formats = ["dd/MM/yyyy"];
    #endregion

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        foreach (var format in _formats)
        {
            if (DateOnly.TryParseExact(value, format, out var date))
            {
                return date;
            }
        }

        throw new JsonException($"Unable to parse '{value}' as DateOnly.");

    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}

