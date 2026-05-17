using Microsoft.AspNetCore.Http;
using NSubstitute;
using Web.Host.Controllers;
using Web.Host.Interfaces;
using Web.Host.Models.Domain;
using Web.Host.Models.Responses.Products;

namespace Web.Host.Test.Controllers;

public class ProductsControllerTests
{
    #region Private Fields
    private readonly IProductService _productServiceMock;
    private readonly ProductsController _sut;
    #endregion

    #region Constructor
    public ProductsControllerTests()
    {
        _productServiceMock = Substitute.For<IProductService>();
        _sut = new ProductsController(_productServiceMock);
    }
    #endregion

    [Fact]
    public async Task GetTopProductsAsync_ReturnsClientResponseWithBody_WhenServiceReturnsData()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);
        var expectedResponse = new GetTopProductsResponse
        {
            Daily = new List<TopProductDaily>
            {
                new TopProductDaily
                {
                    Date = startDate,
                    Product = new Product { Id = "P1", Name = "Test Product" }
                }
            },

            Period = new TopProductPeriod
            {
                StartDate = startDate,
                EndDate = endDate,
                Product = new Product { Id = "P1", Name = "Test Product" }
            }
        };

        _productServiceMock.GetTopProductsAsync(startDate, endDate).Returns(expectedResponse);

        // Act
        var result = await _sut.GetTopProductsAsync(startDate, endDate);

        // Assert
        Assert.True(result.Success);
        Assert.Same(expectedResponse, result.Body);
        await _productServiceMock.Received(1).GetTopProductsAsync(startDate, endDate);
    }

    [Fact]
    public async Task GetTopProductsAsync_UsesDefaultEndDate_WhenEndDateIsDefault()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var defaultEndDate = new DateTime(2026, 4, 23);
        var expectedResponse = new GetTopProductsResponse
        {
            Daily = new List<TopProductDaily>
            {
                new TopProductDaily
                {
                    Date = startDate,
                    Product = new Product { Id = "P1", Name = "Test Product" }
                }
            },

            Period = new TopProductPeriod
            {
                StartDate = startDate,
                EndDate = defaultEndDate,
                Product = new Product { Id = "P1", Name = "Test Product" }
            }
        };

        _productServiceMock.GetTopProductsAsync(startDate, defaultEndDate).Returns(expectedResponse);

        // Act
        var result = await _sut.GetTopProductsAsync(startDate, default);

        // Assert
        Assert.True(result.Success);
        Assert.Same(expectedResponse, result.Body);
        await _productServiceMock.Received(1).GetTopProductsAsync(startDate, defaultEndDate);
    }

    [Fact]
    public async Task GetTopProductsAsync_ThrowsBadHttpRequestException_WhenStartDateAfterEndDate()
    {
        // Arrange
        var startDate = new DateTime(2026, 5, 1);
        var endDate = new DateTime(2026, 4, 1);

        // Act & Assert
        await Assert.ThrowsAsync<BadHttpRequestException>(
            () => _sut.GetTopProductsAsync(startDate, endDate));

        await _productServiceMock.DidNotReceive().GetTopProductsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
    }

}