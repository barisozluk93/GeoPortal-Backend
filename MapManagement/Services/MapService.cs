using MapManagement.DbContexts;
using MapManagement.Entity;
using MapManagement.Interfaces;
using MapManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;

namespace MapManagement.Services
{
    public class MapService : IMapService
    {
        private readonly MapManagementContext _dbContext;

        public MapService(MapManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Layer>> DeleteLayer(long layerId)
        {
            var result = new Result<Layer>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldLayer = await _dbContext.Layers.Where(x => x.Id == layerId && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldLayer != null)
                    {
                        oldLayer.IsDeleted = true;

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldLayer);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<LayerGroup>> DeleteLayerGroup(long layerGroupId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Layer>> EditLayer(Layer layer)
        {
            var result = new Result<Layer>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldLayer = await _dbContext.Layers.Where(x => x.Id == layer.Id && !x.IsDeleted).FirstOrDefaultAsync();

                    if (oldLayer != null)
                    {
                        if (!_dbContext.Layers.Where(x => x.Id != oldLayer.Id && x.Name == oldLayer.Name && !x.IsDeleted).Any())
                        {
                            oldLayer.Name = layer.Name;
                            oldLayer.Url = layer.Url;
                            oldLayer.IsBaseMap = layer.IsBaseMap;

                            transaction.Commit();

                            result.SetData(layer);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Aynı isim ile tanımlı bir layer bulunmaktadır.");
                        }
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<LayerGroup>> EditLayerGroup(LayerGroup layerGroup)
        {
            var result = new Result<LayerGroup>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldLayerGroup = await _dbContext.LayerGroups.Where(x => x.Id == layerGroup.Id && !x.IsDeleted).FirstOrDefaultAsync();

                    if (oldLayerGroup != null)
                    {
                        if (!_dbContext.LayerGroups.Where(x => x.Id != oldLayerGroup.Id && x.Name == oldLayerGroup.Name && !x.IsDeleted).Any())
                        {
                            oldLayerGroup.Name = layerGroup.Name;

                            transaction.Commit();

                            result.SetData(layerGroup);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Aynı isim ile tanımlı bir layer group bulunmaktadır.");
                        }
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<Layer>> GetLayer(long layerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<LayerGroup>> GetLayerGroup(long layerGroupId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<LayerGroup>>> ListLayerGroup()
        {
            var result = new Result<List<LayerGroup>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var groups = await _dbContext.LayerGroups
                        .Include(x => x.Layers)
                        .Where(x => !x.IsDeleted)
                        .ToListAsync();

                    result.SetData(groups);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    {
                        transaction.Rollback();

                        result.SetIsSuccess(false);
                        result.SetMessage(ex.Message);
                    }
                }

                return result;
            }
        }

        public async Task<Result<Layer>> SaveLayer(Layer layer)
        {
            var result = new Result<Layer>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (!_dbContext.Layers.Where(x => x.Name == layer.Name && !x.IsDeleted).Any())
                    {
                        _dbContext.Add(layer);
                        await _dbContext.SaveChangesAsync();

                        transaction.Commit();

                        result.SetData(layer);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Aynı isim ile tanımlı bir layer bulunmaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    {
                        transaction.Rollback();

                        result.SetIsSuccess(false);
                        result.SetMessage(ex.Message);
                    }
                }

                return result;
            }
        }
        public async Task<Result<LayerGroup>> SaveLayerGroup(LayerGroup layerGroup)
        {
            var result = new Result<LayerGroup>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (!_dbContext.LayerGroups.Where(x => x.Name == layerGroup.Name && !x.IsDeleted).Any())
                    {
                        _dbContext.Add(layerGroup);
                        await _dbContext.SaveChangesAsync();

                        transaction.Commit();

                        result.SetData(layerGroup);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Aynı isim ile tanımlı bir layer group bulunmaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    {
                        transaction.Rollback();

                        result.SetIsSuccess(false);
                        result.SetMessage(ex.Message);
                    }
                }

                return result;
            }
        }
    }
}