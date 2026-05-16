namespace Web.Host.Services.Interfaces;

public interface IFileReaderService
{
    Task<TData> ReadAsync<TData>(string path);
}


