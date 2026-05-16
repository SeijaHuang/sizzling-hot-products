using Web.Host.Models.Domain;

namespace Web.Host.Models.Responses.Products;

public class TopProductDaily
{
    public DateOnly Date { set; get; }
    public Product Product { set; get; } = new Product();
}

public class TopProductPeriod
{
    public DateOnly StartDate { set; get; }
    public DateOnly EndDate { set; get; }
    public Product Product { set; get; } = new Product();
}

public class GetTopProductsResponse
{
    public List<TopProductDaily> Daily { set; get; } = new List<TopProductDaily>();
    public TopProductPeriod? Period { set; get; }
}
