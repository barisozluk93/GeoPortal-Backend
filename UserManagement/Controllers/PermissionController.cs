using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Controllers
{
    [Route("/geoPortalApi/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("PermissionScene.Paging.Permission")]
        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, [FromQuery] bool? isDeleted, [FromQuery] string? name, [FromQuery] string? code)
        {
            var result = await _permissionService.Paginate(pagingParameter, isDeleted, name, code);
            return new OkObjectResult(result);
        }

        [HttpGet("All")]
        [Authorize]
        [HasPermission("PermissionScene.All.Permission")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetPermissions();
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("PermissionScene.Save.Permission")]

        public async Task<IActionResult> Save([FromBody] Permission permission)
        {
            var result = await _permissionService.Save(permission);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("PermissionScene.Edit.Permission")]

        public async Task<IActionResult> Update([FromBody] Permission permission)
        {
            var result = await _permissionService.Update(permission);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        [HasPermission("PermissionScene.Delete.Permission")]

        public async Task<IActionResult> Delete(long id)
        {
            var result = await _permissionService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        [HasPermission("PermissionScene.Get.Permission")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _permissionService.GetById(id);
            return new OkObjectResult(result);
        }

        [HttpPost("Export/Excel")]
        [Authorize]
        [HasPermission("Table.Export.Permission")]
        public async Task<IActionResult> ExportExcel()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _permissionService.ExportExcel(token);
            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Yetkiler{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
