using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Entity;
using OrderManagement.Interfaces;
using OrderManagement.Model;

namespace OrderManagement.Controllers
{
    [Route("/geoPortalApi/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("Paginate")]
        [AllowAnonymous]
        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter)
        {
            var result = await _productService.Paginate(pagingParameter);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _productService.GetById(id);
            return new OkObjectResult(result);
        }

        [HttpPost("SmartFilter")]
        [AllowAnonymous]

        public async Task<IActionResult> SmartFilter([FromBody] ProductSmartFilterRequest request)
        {
            var result = await _productService.SmartFilterAsync(request);
            return Ok(result);
        }

    }
}
