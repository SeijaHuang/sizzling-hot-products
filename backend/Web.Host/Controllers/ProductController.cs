using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Web.Host.Models.Responses.Products;
using Web.Host.Services.Interfaces;

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
        public async Task<ActionResult<GetTopProductsResponse>> GetTopProductsAsync([FromQuery] string startDate, [FromQuery] string endDate)
        {
            if (!DateOnly.TryParseExact(startDate, "dd/MM/yyyy", out DateOnly parsedStartDate) || !DateOnly.TryParseExact(endDate, "dd/MM/yyyy", out DateOnly parsedEndDtae))
            {
                return BadRequest();
            }

            var response = await _productService.GetTopProductsAsync(parsedStartDate, parsedEndDtae);

            return Ok(response);
        }
    }
}
