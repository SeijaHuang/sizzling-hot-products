namespace Web.Host.Models.Responses;

    public class ClientError
    {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    }

