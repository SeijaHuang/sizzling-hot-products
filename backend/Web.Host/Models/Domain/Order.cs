
namespace Web.Host.Models.Domain;

public class Order
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderEntry> Entries { get; set; } = new List<OrderEntry>();
    public DateOnly Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

