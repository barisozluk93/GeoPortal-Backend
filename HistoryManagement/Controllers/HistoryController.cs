using HistoryManagement.Interfaces;
using HistoryManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HistoryManagement.Controllers;
[Route("/geoPortalApi/[controller]")]
[ApiController]
[Authorize]
public sealed class HistoryController(IHistoryService historyService) : ControllerBase
{
    [HttpGet("Paginate")]
    public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, [FromQuery] string entityType, [FromQuery] string recordId, [FromQuery] string? serviceName, [FromQuery] string? operationType)
        => Ok(await historyService.Paginate(pagingParameter, entityType, recordId, serviceName, operationType));
}
