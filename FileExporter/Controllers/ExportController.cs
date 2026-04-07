using FileExporter.Authorization;
using FileExporter.Interfaces;
using FileExporter.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.Controllers
{
    [Route("geoPortalApi/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpPost("Excel")]
        [Authorize]
        [HasPermission("Table.Export.Permission")]
        public IActionResult ExportExcel([FromBody] ExportRequestModel request)
        {
            var bytes = _exportService.GenerateExcel(request);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{request.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

    }
}
