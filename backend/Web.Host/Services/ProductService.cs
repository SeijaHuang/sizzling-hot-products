using Ardalis.GuardClauses;
using Web.Host.Models.Domain;
using Web.Host.Models.Responses;
using Web.Host.Models.Responses.Products;
using Web.Host.Services.Interfaces;

namespace Web.Host.Services;

public class ProductService : IProductService
{
    #region Private Fields
    private readonly IFileReaderService<List<Order>> _fileReaderService;
    private readonly string _ordersFilePath = "Data/orders.json";
    private class ProductSalesCount : Dictionary<string, int>;
    private class DailySalesCount : Dictionary<DateOnly, ProductSalesCount>;
    private readonly record struct SeenProduct(string productId, string customerId, DateOnly date);
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

        // This value is used to remove duplicated orders, the fields are productId, customerId, date
        var seen = new HashSet<SeenProduct>();

        // This value is used to count daily sales stats
        var dailyStats = new DailySalesCount();

        foreach (Order order in orders)
        {
            // TODO: cancelled orders logic 
            if (order.Status == "cancelled") continue;
            // If order's date is out of range, skip it
            if (order.Date < startDate || order.Date > endDate) continue;

            if (order.Status == "completed")
            {
                foreach (OrderEntry entry in order.Entries)
                {
                    SeenProduct currentProduct = new SeenProduct(entry.Id, order.CustomerId, order.Date);

                    // Deduplication rules for product sales counting:  
                    // Rule 1: Each product in an order is counted as one sale regardless of quantity
                    //         (e.g. an order with 5 hammers counts as 1 sale, not 5)
                    // Rule 2: If the same customer purchases the same product on the same day across
                    //         multiple orders, only the first occurrence counts as a sale
                    //         (uniqueness is determined by: ProductId + CustomerId + Date)
                    if (seen.Contains(currentProduct)) continue;

                    // If the order is not seen before, we count the sale for the product and add the order to seen set
                    seen.Add(currentProduct);

                    // If the product is not in dailyStats for that date, we create a new entry for that product with count 1, otherwise we increase the count by 1
                    if (!dailyStats.ContainsKey(order.Date))
                    {
                        dailyStats[order.Date] = new ProductSalesCount();
                    }

                    dailyStats[order.Date].TryGetValue(entry.Id, out int currentCount);
                    dailyStats[order.Date][entry.Id] = currentCount + 1;
                }
            }

        }




        throw new NotImplementedException();

    }
}


