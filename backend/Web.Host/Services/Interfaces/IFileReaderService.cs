using System.Text.Json;
namespace Web.Host.Services.Interfaces;

public interface IFileReaderService<TData>
{
    Task<TData> ReadAsync(string path);
}


