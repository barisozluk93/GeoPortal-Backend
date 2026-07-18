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

        public async Task<Result<List<Layer>>> ListLayers()
        {
            var result = new Result<List<Layer>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var groups = await _dbContext.Layers
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
                    var queryable = _dbContext.Layers
                        .Where(x => (!String.IsNullOrEmpty(nameFilter) ? x.Name.ToLower().Contains(nameFilter.ToLower()) : true) &&
                                    (typeFilter.HasValue ? x.Type == (LayerType)typeFilter : true) &&
                                    (!String.IsNullOrEmpty(layerNameFilter) ? x.LayerName.ToLower().Contains(layerNameFilter.ToLower()) : true) &&
                                    (isVisibleFilter.HasValue ? x.IsVisible == isVisibleFilter :  true) &&
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

        private async Task<ExportRequestModel> BuildRequestLayer()
        {
            ExportRequestModel request = null;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var layers = await _dbContext.Layers
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

    }
}