using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [Route("/geoPortalApi/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("OrganizationScene.Paging.Permission")]

        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, [FromQuery] bool? isDeleted, [FromQuery] string? name, [FromQuery] string? taxNo, [FromQuery] string? taxOffice, [FromQuery] string? email, [FromQuery] string? phone)
        {
            var result = await _organizationService.Paginate(pagingParameter, isDeleted, name, taxNo, taxOffice, email, phone);
            return new OkObjectResult(result);
        }

        [HttpGet("All")]
        [AllowAnonymous]
        //[HasPermission("OrganizationScene.Get.Permission")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _organizationService.GetOrganizations();
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("OrganizationScene.Save.Permission")]

        public async Task<IActionResult> Save([FromBody] Organization organization)
        {
            var result = await _organizationService.Save(organization);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("OrganizationScene.Edit.Permission")]

        public async Task<IActionResult> Update([FromBody] Organization organization)
        {
            var result = await _organizationService.Update(organization);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        [HasPermission("OrganizationScene.Delete.Permission")]

        public async Task<IActionResult> Delete(long id)
        {
            var result = await _organizationService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        //[HasPermission("OrganizationScene.Get.Permission")]

        public async Task<IActionResult> GetById(long id)
        {
            var result = await _organizationService.GetById(id);
            return new OkObjectResult(result);
        }

        [HttpPost("Export/Excel")]
        [Authorize]
        [HasPermission("Table.Export.Permission")]
        public async Task<IActionResult> ExportExcel()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _organizationService.ExportExcel(token);
            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Organizasyonlar{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
