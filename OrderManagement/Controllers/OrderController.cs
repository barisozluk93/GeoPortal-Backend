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

        [HttpGet("UpdateStatus/{id}/{status}")]
        [Authorize]
        [HasPermission("OrderScene.Edit.Permission")]
        public async Task<IActionResult> UpdateStatus(long id, long status)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.UpdateStatus(id, status, token);
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
        public async Task<IActionResult> ComingPaginate([FromQuery] PagingParameter pagingParameter, [FromQuery] string? orderNo, [FromQuery] double? priceMin, double? priceMax, [FromQuery] string? orderDateFrom, [FromQuery] string? orderDateTo, [FromQuery] long? orderStatusStr)
        {
            var result = await _orderService.ComingPaginate(pagingParameter, orderNo, priceMin, priceMax, orderDateFrom, orderDateTo, orderStatusStr);
            return new OkObjectResult(result);
        }

        [HttpGet("AddInvoice/{id}/{fileId}")]
        [Authorize]
        [HasPermission("OrderScene.InvoiceSave.Permission")]
        public async Task<IActionResult> AddInvoice(long id, long fileId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.AddInvoice(id, fileId, token);
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

        [HttpPost("Export/Excel")]
        [Authorize]
        [HasPermission("Table.Export.Permission")]
        public async Task<IActionResult> ExportExcel()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _orderService.ExportExcel(token);
            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Sipariþler{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
