using Web.Host.Models.Domain;

namespace Web.Host.Models.Responses.Products;

public class TopProductDaily
{
    public DateTime Date { get; set; }
    public Product Product { get; set; } = new Product();
}

public class TopProductPeriod
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Product Product { get; set; } = new Product();
}

public class GetTopProductsResponse
{
    public List<TopProductDaily> Daily { get; set; } = new List<TopProductDaily>();
    public TopProductPeriod? Period { get; set; }
}
