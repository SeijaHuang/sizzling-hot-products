using Ardalis.GuardClauses;
using Web.Host.Models.Domain;
using Web.Host.Models.Responses;
using Web.Host.Models.Responses.Products;
using Web.Host.Services.Interfaces;

namespace Web.Host.Services;

public class ProductService : IProductService
{
    #region Private Fields
    private readonly IFileReaderService _fileReaderService;
    private readonly string _ordersFilePath = "Data/orders.json";
    private readonly string _productsFilePath = "Data/products.json";
    private readonly string _defaultProductName = "Unknown Product";
    private class ProductSalesCount : Dictionary<string, int>;
    private class DailySalesCount : Dictionary<DateOnly, ProductSalesCount>;
    private readonly record struct SeenProduct(string productId, string customerId, DateOnly date);
    #endregion

    #region Constructor
    public ProductService(IFileReaderService fileReaderService)
    {
        _fileReaderService = Guard.Against.Null(fileReaderService);
    }
    #endregion


    public async Task<ClientResponse<GetTopProductsResponse>> GetTopProductsAsync(DateOnly? startDate, DateOnly? endDate)
    {

        // Read orders from file
        var orders = await _fileReaderService.ReadAsync<List<Order>>(_ordersFilePath);
        var products = await _fileReaderService.ReadAsync<List<Product>>(_productsFilePath);
        var productsDict = products.ToDictionary(p => p.Id, p => p.Name);

        // This value is used to remove duplicated orders, the fields are productId, customerId, date
        var seen = new HashSet<SeenProduct>();

        // This value is used to count daily sales stats
        var dailyStats = new DailySalesCount();
        // This value is used to count total sales stats for the period
        var totalStats = new ProductSalesCount();

        foreach (Order order in orders)
        {
            // If order's date is out of range, skip it
            if (order.Date < startDate || order.Date > endDate) continue;

            // TODO: cancelled orders logic 
            if (order.Status == "cancelled")
            {
                // If the order is cancelled, we need to find the corresponding completed order with the same orderId, and remove the products in that order from seen set and decrease the count in dailyStats
                var cancelledOrder = orders.FirstOrDefault(o => o.OrderId == order.OrderId && o.Status == "completed");
                if (cancelledOrder == null) continue;

                // Remove the products in the cancelled order from seen set and decrease the count in dailyStats
                foreach (OrderEntry entry in cancelledOrder.Entries)
                {
                    var cancelledProduct = new SeenProduct(entry.Id, order.CustomerId, cancelledOrder.Date);
                    seen.Remove(cancelledProduct);

                    if (dailyStats.ContainsKey(cancelledOrder.Date) && dailyStats[cancelledOrder.Date].ContainsKey(entry.Id))
                    {
                        dailyStats[cancelledOrder.Date][entry.Id] = Math.Max(0, dailyStats[cancelledOrder.Date][entry.Id] - 1);
                    }

                    if (totalStats.ContainsKey(entry.Id))
                    {
                        totalStats[entry.Id] = Math.Max(0, totalStats[entry.Id] - 1);
                    }
                }
            }
            ;

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

                    if (!totalStats.ContainsKey(entry.Id))
                    {
                        totalStats[entry.Id] = 0;
                    }
                    totalStats[entry.Id]++;
                }
            }
        }


        var result = new GetTopProductsResponse
        {
            Daily = GetTopProductDailies(dailyStats, productsDict)
        };

        if (endDate > startDate)
        {
            result.Period = GetTopProductPeriod(totalStats, productsDict, startDate.Value, endDate.Value);
        }

        return new ClientResponse<GetTopProductsResponse>
        {
            Body = result,
            Error = null,
            Success = true
        };
    }

    #region Private Methods
    private List<TopProductDaily> GetTopProductDailies(DailySalesCount dailyStats, Dictionary<string, string> productsDict)
    {
        var result = new List<TopProductDaily>();
        foreach (var dailyStat in dailyStats)
        {
            var date = dailyStat.Key;
            var productSales = dailyStat.Value;

            var topProducts = productSales
                .OrderByDescending(item => item.Value)
                .ThenBy(item => productsDict.GetValueOrDefault(item.Key, _defaultProductName))
                .Select(item => new Product
                {
                    Id = item.Key,
                    Name = productsDict.GetValueOrDefault(item.Key, _defaultProductName)
                })
                .First();

            result.Add(new TopProductDaily
            {
                Date = date,
                Product = topProducts
            });
        }
        return result;
    }

    private TopProductPeriod GetTopProductPeriod(ProductSalesCount totalStats, Dictionary<string, string> productsDict, DateOnly startDate, DateOnly endDate)
    {
        var topProduct = totalStats
            .OrderByDescending(item => item.Value)
            .ThenBy(item => productsDict.GetValueOrDefault(item.Key, _defaultProductName))
            .Select(item => new Product
            {
                Id = item.Key,
                Name = productsDict.GetValueOrDefault(item.Key, _defaultProductName)
            })
            .First();

        return new TopProductPeriod
        {
            StartDate = startDate,
            EndDate = endDate,
            Product = topProduct
        };
    }
    #endregion
}


