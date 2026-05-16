using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Host.Interfaces;
using Web.Host.Models.Responses;
using Web.Host.Models.Responses.Products;

namespace Web.Host.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    #region Private Fields
    private readonly IProductService _productService;
    #endregion

    #region Constructor
    public ProductController(IProductService productService)
    {
        _productService = Guard.Against.Null(productService);
    }
    #endregion

    [HttpGet]
    public async Task<ClientResponse<GetTopProductsResponse>> GetTopProductsAsync([FromQuery, Required] DateTime startDate, [FromQuery] DateTime endDate)
    {
        // if endDate is not provided, we will use current date as default value
        if (endDate == default) endDate = new DateTime(2026, 4, 23);

        // Validate that startDate is not later than endDate
        if (startDate > endDate)
        {
            throw new InvalidOperationException("startDate cannot be later than endDate.");
        }

        var response = await _productService.GetTopProductsAsync(startDate, endDate);

        return new ClientResponse<GetTopProductsResponse>
        {
            Success = true,
            Body = response
        };
    }
}

