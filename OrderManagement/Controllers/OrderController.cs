using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Authorization;
using OrderManagement.Entity;
using OrderManagement.Interfaces;
using OrderManagement.Model;

namespace OrderManagement.Controllers
{
    [Route("/geoPortalApi/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("Paginate/{userId}")]
        [Authorize]
        [HasPermission("OrderScene.CustomerPaging.Permission")]
        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, long userId, [FromQuery] string? orderNo, [FromQuery] double? priceMin, double? priceMax, [FromQuery] string? orderDateFrom, [FromQuery] string? orderDateTo, [FromQuery] long? orderStatusStr)
        {
            var result = await _orderService.Paginate(pagingParameter, userId, orderNo, priceMin, priceMax, orderDateFrom, orderDateTo, orderStatusStr);
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("OrderScene.Save.Permission")]
        public async Task<IActionResult> Save([FromBody] Order order)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.Save(order, token);
            return new OkObjectResult(result);
        }

        [HttpPost("UpdateStatus")]
        [Authorize]
        [HasPermission("OrderScene.Edit.Permission")]
        public async Task<IActionResult> UpdateStatus([FromBody] OrderProduct orderProduct)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.UpdateStatus(orderProduct, token);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        [HasPermission("OrderScene.Get.Permission")]
        public async Task<IActionResult> GetById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.GetById(id, token);
            return new OkObjectResult(result);
        }

        [HttpGet("ComingOrder/{id}")]
        [Authorize]
        [HasPermission("OrderScene.Get.Permission")]
        public async Task<IActionResult> GetComingOrderById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.GetComingOrderById(id, token);
            return new OkObjectResult(result);
        }

        [HttpGet("ComingPaginate")]
        [Authorize]
        [HasPermission("OrderScene.Paging.Permission")]
        public async Task<IActionResult> ComingPaginate([FromQuery] PagingParameter pagingParameter, [FromQuery] string? orderNo, [FromQuery] decimal? priceMin, decimal? priceMax, [FromQuery] string? orderDateFrom, [FromQuery] string? orderDateTo, [FromQuery] long? orderStatusStr)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.ComingPaginate(pagingParameter, token, orderNo, priceMin, priceMax, orderDateFrom, orderDateTo, orderStatusStr);
            return new OkObjectResult(result);
        }

        [HttpPost("AddInvoice")]
        [Authorize]
        [HasPermission("OrderScene.InvoiceSave.Permission")]
        public async Task<IActionResult> AddInvoice([FromBody] OrderProduct orderProduct)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.AddInvoice(orderProduct, token);
            return new OkObjectResult(result);
        }

        [HttpDelete("DeleteInvoice/{id}")]
        [Authorize]
        [HasPermission("OrderScene.InvoiceDelete.Permission")]
        public async Task<IActionResult> DeleteInvoice(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.DeleteInvoice(id, token);
            return new OkObjectResult(result);
        }
    }
}
