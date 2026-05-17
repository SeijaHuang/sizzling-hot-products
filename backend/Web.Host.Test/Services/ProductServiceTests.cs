using NSubstitute;
using Web.Host.Interfaces;
using Web.Host.Models.Domain;
using Web.Host.Services;

namespace Web.Host.Test.Services;

public class ProductServiceTests
{
    #region Private Fields
    private readonly IFileReaderService _fileReaderServiceMock;
    private readonly ProductService _sut;
    #endregion

    #region Constructor
    public ProductServiceTests()
    {
        _fileReaderServiceMock = Substitute.For<IFileReaderService>();
        _sut = new ProductService(_fileReaderServiceMock);
    }
    #endregion


    #region Private Methods
    private static List<Product> CreateProducts()
    {
        return new List<Product>
            {
               new Product { Id = "P1", Name = "Banana" },
               new Product { Id = "P2", Name = "Apple" }
            };
    }
    #endregion

    [Fact]
    public async Task GetTopProductsAsync_CountsSameProductOnlyOncePerDay_WhenQuantityAndDuplicateOrdersExist()
    {
        // Arrange
        var orders = new List<Order>
            {
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 2 },
                        new OrderEntry { Id = "P2", Quantity = 5 }
                    }
                },
                 new Order
                {
                    OrderId = "2",
                    CustomerId = "C2",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 1 }
                    }
                },
            };
        var products = CreateProducts();

        _fileReaderServiceMock.ReadAsync<List<Order>>("Data/orders.json").Returns(orders);
        _fileReaderServiceMock.ReadAsync<List<Product>>("Data/products.json").Returns(products);

        // Act
        var result = await _sut.GetTopProductsAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 1));


        // Assert
        Assert.Single(result.Daily);
        Assert.Equal("P1", result.Daily[0].Product.Id);
        Assert.Equal("Banana", result.Daily[0].Product.Name);
        Assert.Null(result.Period);
    }

    [Fact]
    public async Task GetTopProductsAsync_RemovesPreviouslyCountedSale_WhenCancelledOrderExists()
    {
        // Arrange
        var orders = new List<Order>
            {
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 2 },
                        new OrderEntry { Id = "P2", Quantity = 5 }
                    }
                },
                new Order
                {
                    OrderId = "2",
                    CustomerId = "C2",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 1 }
                    }
                },
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "cancelled",
                }
            };

        var products = CreateProducts();

        _fileReaderServiceMock.ReadAsync<List<Order>>("Data/orders.json").Returns(orders);
        _fileReaderServiceMock.ReadAsync<List<Product>>("Data/products.json").Returns(products);

        // Act
        var result = await _sut.GetTopProductsAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 1));

        // Assert
        Assert.Single(result.Daily);
        Assert.Equal("P1", result.Daily[0].Product.Id);
        Assert.Equal("Banana", result.Daily[0].Product.Name);
        Assert.Null(result.Period);
    }

    [Fact]
    public async Task GetTopProductsAsync_RemovesPreviouslyCountedSale_WhenAllOrdersCancelled()
    {
        // Arrange
        var orders = new List<Order>
            {
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 2 },
                        new OrderEntry { Id = "P2", Quantity = 5 }
                    }
                },
                new Order
                {
                    OrderId = "2",
                    CustomerId = "C2",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 1 }
                    }
                },
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "cancelled",
                },
                new Order
                {
                    OrderId = "2",
                    CustomerId = "C2",
                    Date = new DateTime(2026, 1, 1),
                    Status = "cancelled",
                }
            };

        var products = CreateProducts();

        _fileReaderServiceMock.ReadAsync<List<Order>>("Data/orders.json").Returns(orders);
        _fileReaderServiceMock.ReadAsync<List<Product>>("Data/products.json").Returns(products);

        // Act
        var result = await _sut.GetTopProductsAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 1));

        // Assert
        Assert.Empty(result.Daily);
        Assert.Null(result.Period);
    }

    [Fact]
    public async Task GetTopProductsAsync_RemovesPreviouslyCountedSale_WhenAllOrdersCancelled2()
    {
        // Arrange
        var orders = new List<Order>
            {
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 2 },
                        new OrderEntry { Id = "P2", Quantity = 5 }
                    }
                },
                new Order
                {
                    OrderId = "2",
                    CustomerId = "C2",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 1 }
                    }
                },
                new Order
                {
                    OrderId = "3",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 2),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 2 },
                        new OrderEntry { Id = "P2", Quantity = 5 }
                    }
                },
                new Order
                {
                    OrderId = "3",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 2),
                    Status = "cancelled",
                },
            };

        var products = CreateProducts();

        _fileReaderServiceMock.ReadAsync<List<Order>>("Data/orders.json").Returns(orders);
        _fileReaderServiceMock.ReadAsync<List<Product>>("Data/products.json").Returns(products);

        // Act
        var result = await _sut.GetTopProductsAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 2));

        // Assert
        Assert.Single(result.Daily);
    }

    [Fact]
    public async Task GetTopProductsAsync_ChoosesAlphabeticallyFirstProduct_WhenTieOccurs()
    {
        var orders = new List<Order>
            {
                new Order
                {
                    OrderId = "1",
                    CustomerId = "C1",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P1", Quantity = 1 }
                    }
                },
                new Order
                {
                    OrderId = "2",
                    CustomerId = "C2",
                    Date = new DateTime(2026, 1, 1),
                    Status = "completed",
                    Entries = new List<OrderEntry>
                    {
                        new OrderEntry { Id = "P2", Quantity = 1 }
                    }
                }
            };

        var products = CreateProducts();

        _fileReaderServiceMock.ReadAsync<List<Order>>("Data/orders.json").Returns(orders);
        _fileReaderServiceMock.ReadAsync<List<Product>>("Data/products.json").Returns(products);

        var result = await _sut.GetTopProductsAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 1));

        Assert.Single(result.Daily);
        Assert.Equal("P2", result.Daily[0].Product.Id);
        Assert.Equal("Apple", result.Daily[0].Product.Name);
        Assert.Null(result.Period);
    }
}

