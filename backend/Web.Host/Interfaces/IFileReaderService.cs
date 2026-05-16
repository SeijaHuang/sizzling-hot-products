namespace Web.Host.Interfaces;

public interface IFileReaderService
{
    Task<TData> ReadAsync<TData>(string path);
}

