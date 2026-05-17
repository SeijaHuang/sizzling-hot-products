using System.Text.Json;
using Web.Host.Helpers;

namespace Web.Host.Test.Helpers;

public class DateTimeConverterTests
{
    private static JsonSerializerOptions CreateOptions() => new JsonSerializerOptions
    {
        Converters = { new DateTimeConverter() }
    };

    [Fact]
    public void Read_ParsesSupportedDateFormat()
    {
        var options = CreateOptions();
        var json = "\"02/01/2026\"";

        var actual = JsonSerializer.Deserialize<DateTime>(json, options);

        Assert.Equal(new DateTime(2026, 1, 2), actual);
    }

    [Fact]
    public void Read_ThrowsJsonException_ForUnsupportedDateFormat()
    {
        var options = CreateOptions();
        var json = "\"2026-01-02\"";

        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime>(json, options));

        Assert.Contains("Unable to parse", exception.Message);
    }

}

