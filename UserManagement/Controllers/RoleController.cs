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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("RoleScene.Paging.Permission")]

        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, [FromQuery] bool? isDeleted, [FromQuery] string? name)
        {
            var result = await _roleService.Paginate(pagingParameter, isDeleted, name);
            return new OkObjectResult(result);
        }

        [HttpGet("All")]
        [Authorize]
        [HasPermission("RoleScene.All.Permission")]

        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetRoles();
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("RoleScene.Save.Permission")]

        public async Task<IActionResult> Save([FromBody] Role role)
        {
            var result = await _roleService.Save(role);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("RoleScene.Edit.Permission")]

        public async Task<IActionResult> Update([FromBody] Role role)
        {
            var result = await _roleService.Update(role);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        [HasPermission("RoleScene.Delete.Permission")]

        public async Task<IActionResult> Delete(long id)
        {
            var result = await _roleService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        [HasPermission("RoleScene.Get.Permission")]

        public async Task<IActionResult> GetById(long id)
        {
            var result = await _roleService.GetById(id);
            return new OkObjectResult(result);
        }

        [HttpPost("Export/Excel")]
        [Authorize]
        [HasPermission("Table.Export.Permission")]
        public async Task<IActionResult> ExportExcel()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _roleService.ExportExcel(token);
            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Yetkiler{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
