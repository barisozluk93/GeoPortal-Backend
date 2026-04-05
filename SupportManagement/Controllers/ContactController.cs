using Microsoft.AspNetCore.Mvc;
using SupportManagement.Model;
using SupportManagement.Interfaces;

namespace SupportManagement.Controllers;

[ApiController]
[Route("geoPortalApi/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ContactFormRequestModel request)
    {
        var result = await _contactService.CreateTicketFromContactAsync(request);

        return new OkObjectResult(result);
    }
}
