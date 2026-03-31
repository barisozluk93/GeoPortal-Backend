using Microsoft.AspNetCore.Mvc;
using OrderManagement.Interfaces;
using OrderManagement.Model;

namespace OrderManagement.Controllers
{
    [Route("geoPortalApi/[controller]")]
    [ApiController]
    public class InternalApiKeyController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;

        public InternalApiKeyController(IOrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        [HttpPost("Validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateApiKeyRequest request)
        {
            var expectedSecret = _configuration["InternalAuth:Secret"];

            if (!Request.Headers.TryGetValue("X-Internal-Secret", out var secret) ||
                !string.Equals(secret, expectedSecret, StringComparison.Ordinal))
            {
                return Unauthorized();
            }

            var result = await _orderService.ValidateApiKey(request);
            return Ok(result);
        }
    }
}