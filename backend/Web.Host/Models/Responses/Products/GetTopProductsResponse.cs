using Web.Host.Models.Domain;

namespace Web.Host.Models.Responses.Products;

public class TopProductDaily
{
    public DateTime Date { set; get; }
    public Product Product { set; get; } = new Product();
}

public class TopProductPeriod
{
    public DateTime StartDate { set; get; }
    public DateTime EndDate { set; get; }
    public Product Product { set; get; } = new Product();
}

public class GetTopProductsResponse
{
    public List<TopProductDaily> Daily { set; get; } = new List<TopProductDaily>();
    public TopProductPeriod? Period { set; get; }
}
