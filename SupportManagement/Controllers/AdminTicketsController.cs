using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportManagement.Interfaces;
using SupportManagement.Authorization;
using SupportManagement.Model;

namespace SupportBSupportManagementackend.Controllers;

[ApiController]
[Route("geoPortalApi/[controller]")]
public class AdminTicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public AdminTicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet("Paginate")]
    [Authorize]
    [HasPermission("SupportScene.Paging.Permission")]
    public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, [FromQuery] string? ticketNo, [FromQuery] string? customerName, [FromQuery] string? customerEmail, [FromQuery] string? subject, [FromQuery] string? status, [FromQuery] string? lastMessageAtUtcFrom, [FromQuery] string? lastMessageAtUtcTo)
    {
        var result = await _ticketService.Paginate(pagingParameter, ticketNo, customerName, customerEmail, subject, status, lastMessageAtUtcFrom, lastMessageAtUtcTo);
        return new OkObjectResult(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    [HasPermission("SupportScene.Get.Permission")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _ticketService.GetByIdAsync(id);

        return new OkObjectResult(result);
    }

    [HttpPut("{id}/status")]
    [Authorize]
    [HasPermission("SupportScene.Edit.Permission")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateTicketStatusRequestModel request)
    {
        var result = await _ticketService.UpdateStatusAsync(id, request.Status);

        return new OkObjectResult(result);
    }

    [HttpPost("{id}/reply")]
    [Authorize]
    [HasPermission("SupportScene.Reply.Permission")]
    public async Task<IActionResult> Reply(long id, [FromBody] ReplyTicketRequestModel request)
    {
        var result = await _ticketService.ReplyAsync(id, request.AdminEmail, request.Message);

        return new OkObjectResult(result);
    }

    [HttpPost("Export/Excel")]
    [Authorize]
    [HasPermission("Table.Export.Permission")]
    public async Task<IActionResult> ExportExcel()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

        var result = await _ticketService.ExportExcel(token);
        return File(
            result,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Sipariþler{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }
}
