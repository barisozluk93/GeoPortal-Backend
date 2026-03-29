using MapManagement.Entity;
using MapManagement.Model;

namespace MapManagement.Interfaces
{
    public interface IMapService
    {
        Task<Result<List<LayerGroup>>> ListLayerGroup();
        Task<Result<LayerGroup>> SaveLayerGroup(LayerGroup layerGroup);
        Task<Result<LayerGroup>> EditLayerGroup(LayerGroup layerGroup);
        Task<Result<LayerGroup>> DeleteLayerGroup(long layerGroupId);
        Task<Result<LayerGroup>> GetLayerGroup(long layerGroupId);

        Task<Result<Layer>> SaveLayer(Layer layer);
        Task<Result<Layer>> EditLayer(Layer layer);
        Task<Result<Layer>> DeleteLayer(long layerId);
        Task<Result<Layer>> GetLayer(long layerId);
    }
}
