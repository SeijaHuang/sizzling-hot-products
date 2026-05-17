using System.Text.Json;
using Web.Host.Models.Domain;
using Web.Host.Services;

namespace Web.Host.Test.Services;

public class FileReaderServiceTests
{
    #region Private Fields
    private readonly FileReaderService _sut;
    private readonly List<string> _tempFiles = new();
    #endregion

    #region Constructor
    public FileReaderServiceTests()
    {
        _sut = new FileReaderService();
    }
    #endregion

    #region Private Methods
    private string CreateTempJsonFile(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"FileReaderServiceTests_{Guid.NewGuid():N}.json");
        File.WriteAllText(path, content);
        _tempFiles.Add(path);
        return path;
    }
    #endregion

    [Fact]
    public async Task ReadAsync_ReturnsTypedData_WhenFileContainsValidJson()
    {
        // Arrange
        var expected = new List<Product>
            {
                new Product { Id = "P1", Name = "Drill" },
                new Product { Id = "P2", Name = "Saw" }
            };

        var json = JsonSerializer.Serialize(expected);
        var path = CreateTempJsonFile(json);

        // Act
        var actual = await _sut.ReadAsync<List<Product>>(path);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
        Assert.Equal("P1", actual[0].Id);
        Assert.Equal("Drill", actual[0].Name);
    }

    [Fact]
    public async Task ReadAsync_ReadsCaseInsensitivePropertyNames_AndParsesDatesWithCustomConverter()
    {
        // Arrange
        var json = "[" +
                   "{\"orderId\":\"1\",\"customerId\":\"C1\",\"date\":\"02/01/2026\",\"status\":\"completed\",\"entries\":[{\"id\":\"P1\",\"quantity\":1}]}" +
                   "]";
        var path = CreateTempJsonFile(json);

        // Act
        var actual = await _sut.ReadAsync<List<Order>>(path);

        // Assert
        Assert.Single(actual);
        Assert.Equal("1", actual[0].OrderId);
        Assert.Equal("C1", actual[0].CustomerId);
        Assert.Equal(new DateTime(2026, 1, 2), actual[0].Date);
        Assert.Equal("completed", actual[0].Status);
        var entry = Assert.Single(actual[0].Entries!);
        Assert.Equal("P1", entry.Id);
        Assert.Equal(1, entry.Quantity);
    }

    [Fact]
    public async Task ReadAsync_ThrowsFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.json");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(() => _sut.ReadAsync<List<Product>>(path));

        Assert.Contains("The file", exception.Message);
        Assert.Contains(path, exception.Message);
    }

    [Fact]
    public async Task ReadAsync_ThrowsInvalidOperationException_WhenJsonIsMalformed()
    {
        // Arrange
        var path = CreateTempJsonFile("{ invalid json }");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ReadAsync<List<Product>>(path));

        Assert.Contains("Error deserializing JSON data", exception.Message);
    }

    [Fact]
    public async Task ReadAsync_ThrowsInvalidOperationException_WhenJsonDeserializesToNull()
    {
        // Arrange
        var path = CreateTempJsonFile("null");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ReadAsync<List<Product>>(path));

        Assert.Contains("Failed to deserialize data", exception.Message);
    }
}

