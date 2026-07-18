using MapManagement.Entity;
using MapManagement.Model;

namespace MapManagement.Interfaces
{
    public interface IMapService
    {
        Task<Result<List<Layer>>> ListLayers();
        Task<Result<PagingResult<PagedList<Layer>>>> PaginateLayer(PagingParameter pagingParameter, string? nameFilter, long? typeFilter, string? layerNameFilter, bool? isVisibleFilter, string? layerGroupNameFilter, long? orderNoFilter, bool? isDeletedFilter);
        Task<Result<Layer>> SaveLayer(Layer layer);
        Task<Result<Layer>> EditLayer(Layer layer);
        Task<Result<Layer>> DeleteLayer(long layerId);
        Task<Result<Layer>> GetLayerById(long layerId);
        Task<byte[]> ExportLayerExcel(string token);
    }
}
