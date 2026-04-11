using MapManagement.Entity;
using MapManagement.Model;

namespace MapManagement.Interfaces
{
    public interface IMapService
    {
        Task<Result<List<LayerGroup>>> ListLayerGroup();
        Task<Result<List<LayerGroup>>> GetLayerGroups();
        Task<Result<PagingResult<PagedList<LayerGroup>>>> PaginateLayerGroup(PagingParameter pagingParameter, bool? isDeletedFilter, string? nameFilter, long? orderNoFilter);
        Task<Result<LayerGroup>> SaveLayerGroup(LayerGroup layerGroup);
        Task<Result<LayerGroup>> EditLayerGroup(LayerGroup layerGroup);
        Task<Result<LayerGroup>> DeleteLayerGroup(long layerGroupId);
        Task<Result<LayerGroup>> GetLayerGroupById(long layerGroupId);
        Task<byte[]> ExportLayerGroupExcel(string token);
        Task<Result<PagingResult<PagedList<Layer>>>> PaginateLayer(PagingParameter pagingParameter, string? nameFilter, long? typeFilter, string? layerNameFilter, bool? isVisibleFilter, string? layerGroupNameFilter, long? orderNoFilter, bool? isDeletedFilter);
        Task<Result<Layer>> SaveLayer(Layer layer);
        Task<Result<Layer>> EditLayer(Layer layer);
        Task<Result<Layer>> DeleteLayer(long layerId);
        Task<Result<Layer>> GetLayerById(long layerId);
        Task<byte[]> ExportLayerExcel(string token);
    }
}
