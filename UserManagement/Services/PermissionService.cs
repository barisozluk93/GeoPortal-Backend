using Microsoft.EntityFrameworkCore;
using System.Data;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class PermissionService : IPermissionService
    { 
        private readonly UserManagementContext _dbContext;
        private readonly IExportGateway _exportGateway;
        public PermissionService(UserManagementContext dbContext, IExportGateway exportGateway)
        {
            _dbContext = dbContext;
            _exportGateway = exportGateway;
        }

        public async Task<byte[]> ExportExcel(string token)
        {
            var request = await BuildPermissionExportRequest();
            return await _exportGateway.ExportExcelAsync(request, token);
        }

        public async Task<Result<PagingResult<PagedList<Permission>>>> Paginate(PagingParameter pagingParameter, bool? isDeletedFilter, string? nameFilter, string? codeFilter)
        {
            var result = new Result<PagingResult<PagedList<Permission>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = _dbContext.Permissions
                                .Where(x => (isDeletedFilter.HasValue ? x.IsDeleted == isDeletedFilter : true) &&
                                            (!String.IsNullOrEmpty(nameFilter) ? x.Name.ToLower().Contains(nameFilter.ToLower()) : true) &&
                                            (!String.IsNullOrEmpty(codeFilter) ? x.Code.ToLower().Contains(codeFilter.ToLower()) : true));
                    var pagination = PagedList<Permission>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<Permission>> ()
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

        public async Task<Result<List<Permission>>> GetPermissions()
        {
            var result = new Result<List<Permission>>();
            
            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var data = await _dbContext.Permissions.Where(x => !x.IsDeleted).ToListAsync();

                    result.SetData(data);
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

        public async Task<Result<Permission>> Save(Permission permission)
        {
            var result = new Result<Permission>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (!_dbContext.Permissions.Where(x => (x.Name == permission.Name || x.Code == permission.Code) && !x.IsDeleted).Any())
                    {
                        _dbContext.Add(permission);
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(permission);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Aynı isim veya kodla tanımlı bir yetki bulunmaktadır.");
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

        public async Task<Result<Permission>> Update(Permission permission)
        {
            var result = new Result<Permission>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldPermission = await _dbContext.Permissions.Where(x => x.Id == permission.Id && !x.IsDeleted).FirstOrDefaultAsync();

                    if (oldPermission != null)
                    {
                        if (!_dbContext.Permissions.Where(x => x.Id != oldPermission.Id && (x.Name == permission.Name || x.Code == permission.Code) && !x.IsDeleted).Any())
                        {
                            oldPermission.Name = permission.Name;
                            oldPermission.Code = permission.Code;

                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();

                            result.SetData(permission);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Aynı isim veya kodla tanımlı bir yetki bulunmaktadır.");
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

        public async Task<Result<Permission>> Delete(long id)
        {
            var result = new Result<Permission>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldPermission = await _dbContext.Permissions.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldPermission != null)
                    {
                        oldPermission.IsDeleted = true;

                        var rolePermissions = await _dbContext.RolePermissions.Where(x => x.PermissionId == oldPermission.Id).ToListAsync();
                        _dbContext.RolePermissions.RemoveRange(rolePermissions);


                        var userPermissions = await _dbContext.UserPermissions.Where(x => x.PermissionId == oldPermission.Id).ToListAsync();
                        _dbContext.UserPermissions.RemoveRange(userPermissions);

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldPermission);
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

        public async Task<Result<Permission>> GetById(long id)
        {
            var result = new Result<Permission>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var permission = await _dbContext.Permissions.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (permission != null)
                    {
                        result.SetData(permission);
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
                }
            }

            return result;
        }

        private async Task<ExportRequestModel> BuildPermissionExportRequest()
        {
            ExportRequestModel request = null;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var permissions = await _dbContext.Permissions.ToListAsync();

                    request = new ExportRequestModel
                    {
                        FileName = "Yetkiler",
                        SheetName = "Yetkiler",
                        Title = "Yetki Listesi",
                        Columns = new List<ExportColumnModel>
                    {
                        new() { Key = "Id", Header = "Id", Width = 10, DataType = "number" },
                        new() { Key = "Name", Header = "Ad", Width = 30, DataType = "text" },
                        new() { Key = "Code", Header = "Kod", Width = 35, DataType = "text" },
                        new() { Key = "IsDeleted", Header = "Silindi Mi?", Width = 15, DataType = "boolean" },
                        new() { Key = "IsSystemData", Header = "Sistem Verisi Mi?", Width = 18, DataType = "boolean" }
                    },
                        Rows = permissions.Select(x => new Dictionary<string, object?>
                        {
                            ["Id"] = x.Id,
                            ["Name"] = x.Name,
                            ["Code"] = x.Code,
                            ["IsDeleted"] = x.IsDeleted,
                            ["IsSystemData"] = x.IsSystemData
                        }).ToList()
                    };
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
