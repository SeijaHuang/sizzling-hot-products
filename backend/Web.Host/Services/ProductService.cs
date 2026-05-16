using Ardalis.GuardClauses;
using Web.Host.Interfaces;
using Web.Host.Models.Domain;
using Web.Host.Models.Responses.Products;

namespace Web.Host.Services;

public class ProductService : IProductService
{
    #region Private Fields
    private readonly IFileReaderService _fileReaderService;
    private readonly string _ordersFilePath = "Data/orders.json";
    private readonly string _productsFilePath = "Data/products.json";
    private readonly string _defaultProductName = "Unknown Product";
    private class SalesCountByProduct : Dictionary<string, int>; // key is productId, value is sales count
    private class DailyProductSales : Dictionary<DateTime, SalesCountByProduct>;
    private readonly record struct UniqueSaleKey(string productId, string customerId, DateTime date);
    #endregion

    #region Constructor
    public ProductService(IFileReaderService fileReaderService)
    {
        _fileReaderService = Guard.Against.Null(fileReaderService);
    }
    #endregion

    public async Task<GetTopProductsResponse> GetTopProductsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {   // Read orders from file
            var orders = await _fileReaderService.ReadAsync<List<Order>>(_ordersFilePath);
            var products = await _fileReaderService.ReadAsync<List<Product>>(_productsFilePath);

            var productsDict = products.ToDictionary(p => p.Id, p => p.Name);
            // Create a dictionary to store completed orders with orderId as key, this will be used to handle cancelled orders

            var completedOrders = orders
            .Where(o => o.Status == "completed")
            .ToDictionary(o => o.OrderId, o => o);

            // This value is used to remove duplicated orders, the fields are productId, customerId, date
            var seen = new HashSet<UniqueSaleKey>();

            // This value is used to count daily sales stats
            var dailyStats = new DailyProductSales();
            // This value is used to count total sales stats for the period
            var totalStats = new SalesCountByProduct();

            foreach (Order order in orders)
            {
                // If order's date is out of range, skip it
                if (order.Date < startDate || order.Date > endDate) continue;

                if (order.Status == "cancelled")
                {
                    // If the order is cancelled, we need to find the corresponding completed order with the same orderId, and remove the products in that order from seen set and decrease the count in dailyStats
                    var cancelledOrder = completedOrders.GetValueOrDefault(order.OrderId);
                    if (cancelledOrder == null) continue;

                    // Remove the products in the cancelled order from seen set and decrease the count in dailyStats
                    foreach (OrderEntry entry in cancelledOrder.Entries)
                    {
                        var cancelledProduct = new UniqueSaleKey(entry.Id, cancelledOrder.CustomerId, cancelledOrder.Date);
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
                        UniqueSaleKey currentProduct = new UniqueSaleKey(entry.Id, order.CustomerId, order.Date);

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
                            dailyStats[order.Date] = new SalesCountByProduct();
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
                result.Period = GetTopProductPeriod(totalStats, productsDict, startDate, endDate);
            }

            return result;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    #region Private Methods
    private List<TopProductDaily> GetTopProductDailies(DailyProductSales dailyStats, Dictionary<string, string> productsDict)
    {
        var result = new List<TopProductDaily>();
        foreach (var dailyStat in dailyStats)
        {
            var date = dailyStat.Key;
            var productSales = dailyStat.Value;

            var topProducts = GetTopProduct(productSales, productsDict);

            if (topProducts == null) continue;

            result.Add(new TopProductDaily
            {
                Date = date,
                Product = topProducts
            });
        }
        return result;
    }

    private TopProductPeriod GetTopProductPeriod(SalesCountByProduct totalStats, Dictionary<string, string> productsDict, DateTime startDate, DateTime endDate)
    {
        var topProduct = GetTopProduct(totalStats, productsDict);

        return new TopProductPeriod
        {
            StartDate = startDate,
            EndDate = endDate,
            Product = topProduct ?? new Product()
        };
    }

    private Product? GetTopProduct(SalesCountByProduct productSales, Dictionary<string, string> productsDict)
    {
        var topProduct = productSales
            .OrderByDescending(item => item.Value)
            .ThenBy(item => productsDict.GetValueOrDefault(item.Key, _defaultProductName))
            .Select(item => new Product
            {
                Id = item.Key,
                Name = productsDict.GetValueOrDefault(item.Key, _defaultProductName)
            })
            .FirstOrDefault();
        return topProduct;
    }

    #endregion
}


