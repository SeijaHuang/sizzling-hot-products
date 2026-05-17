using System.Text.Json;
using Web.Host.Helpers;
using Web.Host.Interfaces;

namespace Web.Host.Services;

public class FileReaderService : IFileReaderService
{
    #region Private Fields
    private readonly JsonSerializerOptions _defaultOptions;
    #endregion

    #region Constructor
    public FileReaderService(JsonSerializerOptions? defaultOptions = null)
    {
        _defaultOptions = defaultOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateTimeConverter() }
        };
    }
    #endregion

    public async Task<TData> ReadAsync<TData>(string path)
    {
        try
        {
            await using var file = File.OpenRead(path);
            var data = await JsonSerializer.DeserializeAsync<TData>(file, _defaultOptions);
            if (data == null)
            {
                throw new InvalidOperationException($"Failed to deserialize data from file '{path}'.");
            }
            return data;
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException($"The file '{path}' was not found.", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Error deserializing JSON data from file '{path}': {ex.Message}", ex);
        }
    }
}
