using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Host.Interfaces;
using Web.Host.Models.Responses.Products;

namespace Web.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region Pivate Fields
        private readonly IProductService _productService;
        #endregion

        #region Constructor
        public ProductController(IProductService productService)
        {
            _productService = Guard.Against.Null(productService);
        }
        #endregion

        [HttpGet]
        public async Task<ActionResult<GetTopProductsResponse>> GetTopProductsAsync([FromQuery, Required] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate == default) endDate = new DateTime(2026, 4, 23);

            var response = await _productService.GetTopProductsAsync(startDate, endDate);

            return Ok(response);
        }
    }
}
