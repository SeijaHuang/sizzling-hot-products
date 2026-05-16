using Web.Host.Models.Responses.Products;

namespace Web.Host.Interfaces;

public interface IProductService
{
    Task<GetTopProductsResponse> GetTopProductsAsync(DateTime startDate, DateTime endDate);
}

