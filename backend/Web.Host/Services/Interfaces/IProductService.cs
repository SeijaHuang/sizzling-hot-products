using Web.Host.Models.Responses;
using Web.Host.Models.Responses.Products;

namespace Web.Host.Services.Interfaces;

public interface IProductService
{
    Task<ClientResponse<GetTopProductsResponse>> GetTopProductsAsync(DateOnly? startDate, DateOnly? endDate);
}


