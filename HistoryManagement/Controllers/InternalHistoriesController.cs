using HistoryManagement.Entities;
using HistoryManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HistoryManagement.Controllers;
[Route("internal/histories")]
[ApiController]
[AllowAnonymous]
public sealed class InternalHistoriesController(IHistoryService historyService, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] HistoryRecord history)
    {
        var expected = configuration["InternalAuth:ApiKey"];
        var actual = Request.Headers["X-Internal-Api-Key"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(expected) || actual != expected) return Unauthorized();
        var result = await historyService.Save(history);
        return result.GetIsSuccess() == true ? Ok(result) : BadRequest(result);
    }
}
