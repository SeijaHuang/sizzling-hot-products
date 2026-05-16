using Web.Host.Models.Domain;
using Web.Host.Models.Responses;
using Web.Host.Models.Responses.Products;
using Web.Host.Services.Interfaces;
using Ardalis.GuardClauses;

namespace Web.Host.Services;

public class ProductService : IProductService
{
    #region Private Fields
    private readonly IFileReaderService<List<Order>> _fileReaderService;
    private readonly string _ordersFilePath = "Data/orders.json";
    #endregion

    #region Constructor
    public ProductService(IFileReaderService<List<Order>> fileReaderService)
    {
        _fileReaderService = Guard.Against.Null(fileReaderService);
    }
    #endregion


    public async Task<ClientResponse<GetTopProductsResponse>> GetTopProductsAsync(DateOnly? startDate, DateOnly? endDate)
    {

        // Read orders from file
        var orders = await _fileReaderService.ReadAsync(_ordersFilePath);

        throw new NotImplementedException();

    }
}


