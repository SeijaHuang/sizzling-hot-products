namespace Web.Host.Models.Responses;

    public class ClientResponse<TBody>
    {
    public bool Success { get; set; }
    public TBody? Body { get; set; }
    public ClientError? Error { get; set; }
    }

