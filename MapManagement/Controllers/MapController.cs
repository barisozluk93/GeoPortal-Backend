using MapManagement.Authorization;
using MapManagement.Entity;
using MapManagement.Interfaces;
using MapManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> ListLayerGroup()
        {
            var result = await _mapService.ListLayerGroup();
            return new OkObjectResult(result);
        }

        [HttpGet("GetLayerGroups")]
        [Authorize]
        [HasPermission("LayerGroupScene.All.Permission")]
        public async Task<IActionResult> GetLayerGroups()
        {
            var result = await _mapService.ListLayerGroup();
            return new OkObjectResult(result);
        }


        [HttpGet("PaginateLayerGroup")]
        [Authorize]
        [HasPermission("LayerGroupScene.Paging.Permission")]
        public async Task<IActionResult> PaginateLayerGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] bool? isDeleted, [FromQuery] string? name, [FromQuery] long? orderNo)
        {
            var result = await _mapService.PaginateLayerGroup(pagingParameter, isDeleted, name, orderNo);
            return new OkObjectResult(result);
        }

        [HttpPost("SaveLayerGroup")]
        [Authorize]
        [HasPermission("LayerGroupScene.Save.Permission")]
        public async Task<IActionResult> SaveLayerGroup([FromBody] LayerGroup layerGroup)
        {
            var result = await _mapService.SaveLayerGroup(layerGroup);
            return new OkObjectResult(result);
        }

        [HttpPost("EditLayerGroup")]
        [Authorize]
        [HasPermission("LayerGroupScene.Edit.Permission")]
        public async Task<IActionResult> EditLayerGroup([FromBody] LayerGroup layerGroup)
        {
            var result = await _mapService.EditLayerGroup(layerGroup);
            return new OkObjectResult(result);
        }

        [HttpDelete("DeleteLayerGroup/{layerGroupId}")]
        [Authorize]
        [HasPermission("LayerGroupScene.Delete.Permission")]
        public async Task<IActionResult> DeleteLayerGroup(long layerGroupId)
        {
            var result = await _mapService.DeleteLayerGroup(layerGroupId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetLayerGroupById/{layerGroupId}")]
        [Authorize]
        [HasPermission("LayerGroupScene.Get.Permission")]
        public async Task<IActionResult> GetLayerGroupById(long layerGroupId)
        {
            var result = await _mapService.GetLayerGroupById(layerGroupId);
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
    }
}
