using MapManagement.Authorization;
using MapManagement.Entity;
using MapManagement.Interfaces;
using MapManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;

namespace FileManagement.Controllers
{
    [Route("geoPortalApi/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IMapService _mapService;

        public MapController(IMapService mapService)
        {
            _mapService = mapService;
        }

        [HttpGet("LayerList")]
        [AllowAnonymous]
        public async Task<IActionResult> ListLayers()
        {
            var result = await _mapService.ListLayers();
            return new OkObjectResult(result);
        }

        [HttpGet("PaginateLayer")]
        [Authorize]
        [HasPermission("LayerScene.Paging.Permission")]
        public async Task<IActionResult> PaginateLayer([FromQuery] PagingParameter pagingParameter, [FromQuery] string? name, [FromQuery] long? type, [FromQuery] string? layerName, [FromQuery] bool? isVisible, [FromQuery] string? layerGroupName, [FromQuery] long? orderNo, [FromQuery] bool? isDeleted)
        {
            var result = await _mapService.PaginateLayer(pagingParameter, name, type, layerName, isVisible, layerGroupName, orderNo, isDeleted);
            return new OkObjectResult(result);
        }


        [HttpPost("SaveLayer")]
        [Authorize]
        [HasPermission("LayerScene.Save.Permission")]
        public async Task<IActionResult> SaveLayer([FromBody] Layer layer)
        {
            var result = await _mapService.SaveLayer(layer);
            return new OkObjectResult(result);
        }

        [HttpPost("EditLayer")]
        [Authorize]
        [HasPermission("LayerScene.Edit.Permission")]
        public async Task<IActionResult> EditLayer([FromBody] Layer layer)
        {
            var result = await _mapService.EditLayer(layer);
            return new OkObjectResult(result);
        }

        [HttpDelete("DeleteLayer/{layerId}")]
        [Authorize]
        [HasPermission("LayerScene.Delete.Permission")]
        public async Task<IActionResult> DeleteLayer(long layerId)
        {
            var result = await _mapService.DeleteLayer(layerId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetLayer/{layerId}")]
        [Authorize]
        [HasPermission("LayerScene.Get.Permission")]
        public async Task<IActionResult> GetLayerById(long layerId)
        {
            var result = await _mapService.GetLayerById(layerId);
            return new OkObjectResult(result);
        }

        [HttpPost("ExportLayer/Excel")]
        [Authorize]
        [HasPermission("Table.Export.Permission")]
        public async Task<IActionResult> ExportLayerExcel()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _mapService.ExportLayerExcel(token);
            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Katmanlar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
