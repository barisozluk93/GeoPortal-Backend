using MapManagement.Authorization;
using MapManagement.Entity;
using MapManagement.Interfaces;
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

        [HttpPost("SaveLayerGroup")]
        [Authorize]
        [HasPermission("MapScene.LayerGroupSave.Permission")]
        public async Task<IActionResult> SaveLayerGroup([FromBody] LayerGroup layerGroup)
        {
            var result = await _mapService.SaveLayerGroup(layerGroup);
            return new OkObjectResult(result);
        }

        [HttpPost("EditLayerGroup")]
        [Authorize]
        [HasPermission("MapScene.LayerGroupEdit.Permission")]
        public async Task<IActionResult> EditLayerGroup([FromBody] LayerGroup layerGroup)
        {
            var result = await _mapService.EditLayerGroup(layerGroup);
            return new OkObjectResult(result);
        }

        [HttpDelete("DeleteLayerGroup/{layerGroupId}")]
        [Authorize]
        [HasPermission("MapScene.LayerGroupDelete.Permission")]
        public async Task<IActionResult> DeleteLayerGroup(long layerGroupId)
        {
            var result = await _mapService.DeleteLayerGroup(layerGroupId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetLayerGroup/{layerGroupId}")]
        [Authorize]
        [HasPermission("MapScene.LayerGroupGet.Permission")]
        public async Task<IActionResult> GetLayerGroup(long layerGroupId)
        {
            var result = await _mapService.GetLayerGroup(layerGroupId);
            return new OkObjectResult(result);
        }

        [HttpPost("SaveLayer")]
        [Authorize]
        [HasPermission("MapScene.LayerSave.Permission")]
        public async Task<IActionResult> SaveLayer([FromBody] Layer layer)
        {
            var result = await _mapService.SaveLayer(layer);
            return new OkObjectResult(result);
        }

        [HttpPost("EditLayer")]
        [Authorize]
        [HasPermission("MapScene.LayerEdit.Permission")]
        public async Task<IActionResult> EditLayer([FromBody] Layer layer)
        {
            var result = await _mapService.EditLayer(layer);
            return new OkObjectResult(result);
        }

        [HttpDelete("DeleteLayer/{layerId}")]
        [Authorize]
        [HasPermission("MapScene.LayerDelete.Permission")]
        public async Task<IActionResult> DeleteLayer(long layerId)
        {
            var result = await _mapService.DeleteLayer(layerId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetLayer/{layerId}")]
        [Authorize]
        [HasPermission("MapScene.LayerGet.Permission")]
        public async Task<IActionResult> GetLayer(long layerId)
        {
            var result = await _mapService.GetLayer(layerId);
            return new OkObjectResult(result);
        }
    }
}
