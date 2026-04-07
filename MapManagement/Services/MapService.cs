using MapManagement.DbContexts;
using MapManagement.Entity;
using MapManagement.Enums;
using MapManagement.Interfaces;
using MapManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MapManagement.Services
{
    public class MapService : IMapService
    {
        private readonly MapManagementContext _dbContext;

        private readonly IExportGateway _exportGateway;
        public MapService(MapManagementContext dbContext, IExportGateway exportGateway)
        {
            _dbContext = dbContext;
            _exportGateway = exportGateway;
        }

        public async Task<byte[]> ExportLayerGroupExcel(string token)
        {
            var request = await BuildRequestLayerGroup();
            return await _exportGateway.ExportExcelAsync(request, token);
        }

        public async Task<byte[]> ExportLayerExcel(string token)
        {
            var request = await BuildRequestLayer();
            return await _exportGateway.ExportExcelAsync(request, token);
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
            var result = new Result<LayerGroup>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldLayerGroup = await _dbContext.LayerGroups.Where(x => x.Id == layerGroupId && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldLayerGroup != null)
                    {
                        oldLayerGroup.IsDeleted = true;

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldLayerGroup);
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
                            oldLayer.Opacity = layer.Opacity;
                            oldLayer.Version = layer.Version;
                            oldLayer.IsVisible = layer.IsVisible;
                            oldLayer.LayerGroupId = layer.LayerGroupId;
                            oldLayer.LayerName = layer.LayerName;
                            oldLayer.OrderNo = layer.OrderNo;
                            oldLayer.Type = layer.Type;
                            oldLayer.Format = layer.Format;

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
                            oldLayerGroup.OrderNo = layerGroup.OrderNo;
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

        public async Task<Result<Layer>> GetLayerById(long layerId)
        {
            var result = new Result<Layer>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var layer = await _dbContext.Layers.Where(x => x.Id == layerId && !x.IsDeleted).FirstOrDefaultAsync();

                    if (layer != null)
                    {
                        transaction.Commit();

                        result.SetData(layer);
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

        public async Task<Result<LayerGroup>> GetLayerGroupById(long layerGroupId)
        {
            var result = new Result<LayerGroup>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var layerGroup = await _dbContext.LayerGroups.Where(x => x.Id == layerGroupId && !x.IsDeleted).FirstOrDefaultAsync();

                    if (layerGroup != null)
                    {
                        transaction.Commit();

                        result.SetData(layerGroup);
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

        public async Task<Result<List<LayerGroup>>> ListLayerGroup()
        {
            var result = new Result<List<LayerGroup>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var groups = await _dbContext.LayerGroups
                        .Include(x => x.Layers.Where(x => !x.IsDeleted).OrderBy(o => o.OrderNo))
                        .Where(x => !x.IsDeleted)
                        .OrderBy(o => o.OrderNo)
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

        public async Task<Result<PagingResult<PagedList<Layer>>>> PaginateLayer(PagingParameter pagingParameter, string? nameFilter, long? typeFilter, string? layerNameFilter, bool? isVisibleFilter, string? layerGroupNameFilter, long? orderNoFilter, bool? isDeletedFilter)
        {
            var result = new Result<PagingResult<PagedList<Layer>>>();


            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = _dbContext.Layers.Include(x => x.LayerGroup)
                        .Where(x => (!String.IsNullOrEmpty(nameFilter) ? x.Name.ToLower().Contains(nameFilter.ToLower()) : true) &&
                                    (typeFilter.HasValue ? x.Type == (LayerType)typeFilter : true) &&
                                    (!String.IsNullOrEmpty(layerNameFilter) ? x.LayerName.ToLower().Contains(layerNameFilter.ToLower()) : true) &&
                                    (isVisibleFilter.HasValue ? x.IsVisible == isVisibleFilter :  true) &&
                                    (!String.IsNullOrEmpty(layerGroupNameFilter) ? x.LayerGroup.Name.ToLower().Contains(layerGroupNameFilter.ToLower()) : true) &&
                                    (orderNoFilter.HasValue ? x.OrderNo == orderNoFilter : true) &&
                                    (isDeletedFilter.HasValue ? x.IsDeleted == isDeletedFilter : true))
                        .Select(s => new Layer
                        {
                            Id = s.Id,
                            Name = s.Name,
                            OrderNo = s.OrderNo,
                            Format = s.Format,
                            IsVisible = s.IsVisible,
                            LayerName = s.LayerName,
                            Opacity = s.Opacity,
                            Type = s.Type,
                            Version = s.Version,
                            CreatedAt = s.CreatedAt,
                            IsDeleted = s.IsDeleted,
                            Url = s.Url,
                            LayerGroup = s.LayerGroup,
                            LayerGroupId = s.LayerGroupId
                        }).OrderBy(o => o.OrderNo);

                    var pagination = PagedList<Layer>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<Layer>>()
                    {
                        Items = pagination,
                        TotalCount = pagination.TotalCount,
                    });

                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<PagingResult<PagedList<LayerGroup>>>> PaginateLayerGroup(PagingParameter pagingParameter, bool? isDeletedFilter, string? nameFilter, long? orderNoFilter)
        {
            var result = new Result<PagingResult<PagedList<LayerGroup>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = _dbContext.LayerGroups
                        .Where(x => (isDeletedFilter.HasValue ? x.IsDeleted == isDeletedFilter : true) &&
                                    (!String.IsNullOrEmpty(nameFilter) ? x.Name.ToLower().Contains(nameFilter.ToLower()) : true) &&
                                    (orderNoFilter.HasValue ? x.OrderNo == orderNoFilter : true))
                        .Select(s => new LayerGroup
                        {
                            Id = s.Id,
                            Name = s.Name,
                            OrderNo = s.OrderNo,
                            IsDeleted = s.IsDeleted,
                        }).OrderBy(o => o.OrderNo);

                    var pagination = PagedList<LayerGroup>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<LayerGroup>>()
                    {
                        Items = pagination,
                        TotalCount = pagination.TotalCount,
                    });

                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
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


        private async Task<ExportRequestModel> BuildRequestLayer()
        {
            ExportRequestModel request = null;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var layers = await _dbContext.Layers
                    .Include(x => x.LayerGroup)
                    .OrderBy(x => x.OrderNo)
                    .ToListAsync();

                    request = new ExportRequestModel
                    {
                        FileName = "Katmanlar",
                        SheetName = "Katmanlar",
                        Title = "Katman Listesi",
                        Columns = new List<ExportColumnModel>
                        {
                            new() { Key = "Id", Header = "Id", Width = 10, DataType = "number" },
                            new() { Key = "Name", Header = "Ad", Width = 25, DataType = "text" },
                            new() { Key = "Type", Header = "Tip", Width = 15, DataType = "text" },
                            new() { Key = "Url", Header = "Url", Width = 40, DataType = "text" },
                            new() { Key = "LayerName", Header = "Katman Adı", Width = 25, DataType = "text" },
                            new() { Key = "Format", Header = "Format", Width = 15, DataType = "text" },
                            new() { Key = "Version", Header = "Versiyon", Width = 15, DataType = "text" },
                            new() { Key = "IsVisible", Header = "Görünür Mü?", Width = 12, DataType = "boolean"  },
                            new() { Key = "Opacity", Header = "Opaklık", Width = 12, Format = "0.00", DataType = "number" },
                            new() { Key = "OrderNo", Header = "Sıra Numarası", Width = 12, DataType = "number" },
                            new() { Key = "CreatedAt", Header = "Oluşturulma Tarihi", Width = 22, DataType = "text" },
                            new() { Key = "LayerGroup", Header = "Ait Olduğu Katman Grubu", Width = 25, DataType = "text" },
                            new() { Key = "IsDeleted", Header = "Silindi Mi?", Width = 12, DataType = "boolean" }
                        }
                    };

                    request.Rows = layers.Select(x => new Dictionary<string, object?>
                    {
                        ["Id"] = x.Id,
                        ["Name"] = x.Name,
                        ["Type"] = x.Type.ToString(),
                        ["Url"] = x.Url,
                        ["LayerName"] = x.LayerName,
                        ["Format"] = x.Format,
                        ["Version"] = x.Version,
                        ["IsVisible"] = x.IsVisible,
                        ["Opacity"] = x.Opacity,
                        ["OrderNo"] = x.OrderNo,
                        ["CreatedAt"] = x.CreatedAt,
                        ["LayerGroup"] = x.LayerGroup != null ? x.LayerGroup.Name : string.Empty,
                        ["IsDeleted"] = x.IsDeleted
                    }).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            return request;
        }

        private async Task<ExportRequestModel> BuildRequestLayerGroup()
        {
            ExportRequestModel request = null;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var layerGroups = await _dbContext.LayerGroups
                    .Include(x => x.Layers)
                    .OrderBy(x => x.OrderNo)
                    .ToListAsync();

                    request = new ExportRequestModel
                    {
                        FileName = "Katman_Grupları",
                        SheetName = "KatmanGrupları",
                        Title = "Katman Grubu Listesi",
                        Columns = new List<ExportColumnModel>
                        {
                            new() { Key = "Id", Header = "Id", Width = 10, DataType = "number" },
                            new() { Key = "Name", Header = "Ad", Width = 25, DataType = "text" },
                            new() { Key = "OrderNo", Header = "Sıra No", Width = 15 , DataType = "number"},
                            new() { Key = "IsDeleted", Header = "Silindi Mi?", Width = 15, DataType = "boolean" },
                        }
                    };

                    request.Rows = layerGroups.Select(x => new Dictionary<string, object?>
                    {
                        ["Id"] = x.Id,
                        ["Name"] = x.Name,
                        ["OrderNo"] = x.OrderNo,
                        ["IsDeleted"] = x.IsDeleted,
                    }).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return request;
        }
    }
}