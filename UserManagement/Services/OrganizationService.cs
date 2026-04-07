using Microsoft.EntityFrameworkCore;
using System.Data;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly UserManagementContext _dbContext;
        private readonly IExportGateway _exportGateway;
        public OrganizationService(UserManagementContext dbContext, IExportGateway exportGateway)
        {
            _dbContext = dbContext;
            _exportGateway = exportGateway;
        }

        public async Task<byte[]> ExportExcel(string token)
        {
            var request = await BuildOrganizationExportRequest();
            return await _exportGateway.ExportExcelAsync(request, token);
            return null;
        }

        public async Task<Result<PagingResult<PagedList<Organization>>>> Paginate(PagingParameter pagingParameter, bool? isDeletedFilter, string? nameFilter, string? taxNoFilter, string? taxOfficeFilter, string? emailFilter, string? phoneFilter)
        {
            var result = new Result<PagingResult<PagedList<Organization>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = _dbContext.Organizations
                                                .Where(x => (isDeletedFilter.HasValue ? x.IsDeleted == isDeletedFilter : true) &&
                                                            (!String.IsNullOrEmpty(nameFilter) ? (x.Name).ToLower().Contains(nameFilter.ToLower()) : true) &&
                                                            (!String.IsNullOrEmpty(taxNoFilter) ? x.TaxNo.ToLower().Contains(taxNoFilter.ToLower()) : true) &&
                                                            (!String.IsNullOrEmpty(taxOfficeFilter) ? x.TaxOffice.ToLower().Contains(taxOfficeFilter.ToLower()) : true) &&
                                                            (!String.IsNullOrEmpty(emailFilter) ? x.Email.ToLower().Contains(emailFilter.ToLower()) : true) &&
                                                            (!String.IsNullOrEmpty(phoneFilter) ? x.Phone.ToLower().Contains(phoneFilter.ToLower()) : true));
                    var pagination = PagedList<Organization>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<Organization>>()
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

        public async Task<Result<List<Organization>>> GetOrganizations()
        {
            var result = new Result<List<Organization>>();
            
            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var data = await _dbContext.Organizations.Where(x => !x.IsDeleted).ToListAsync();

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

        public async Task<Result<Organization>> Save(Organization organization)
        {
            var result = new Result<Organization>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (!_dbContext.Organizations.Where(x => (x.Name == organization.Name) && !x.IsDeleted).Any())
                    {
                        _dbContext.Add(organization);
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(organization);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Aynı isim ile tanımlı bir organizasyon bulunmaktadır.");
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

        public async Task<Result<Organization>> Update(Organization organization)
        {
            var result = new Result<Organization>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldOrganization = await _dbContext.Organizations.Where(x => x.Id == organization.Id && !x.IsDeleted).FirstOrDefaultAsync();

                    if (oldOrganization != null)
                    {
                        if (!_dbContext.Organizations.Where(x => x.Id != oldOrganization.Id && x.Name == organization.Name && !x.IsDeleted).Any())
                        {
                            oldOrganization.Name = organization.Name;
                            oldOrganization.TaxOffice = organization.TaxOffice;
                            oldOrganization.TaxNo = organization.TaxNo;
                            oldOrganization.Email = organization.Email;
                            oldOrganization.Phone = organization.Phone; 

                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();

                            result.SetData(organization);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Aynı isim ile tanımlı bir organizasyon bulunmaktadır.");
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

        public async Task<Result<Organization>> Delete(long id)
        {
            var result = new Result<Organization>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldOrganization = await _dbContext.Organizations.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldOrganization != null)
                    {
                        oldOrganization.IsDeleted = true;

                        var users = await _dbContext.OrganizationUsers.Where(x => x.OrganizationId == oldOrganization.Id).ToListAsync();

                        foreach (var user in users)
                        {
                            user.IsDeleted = true;
                        }
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldOrganization);
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

        public async Task<Result<Organization>> GetById(long id)
        {
            var result = new Result<Organization>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var Organization = await _dbContext.Organizations.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (Organization != null)
                    {
                        result.SetData(Organization);
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

        private async Task<ExportRequestModel> BuildOrganizationExportRequest()
        {
            ExportRequestModel request = null;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var organizations = await _dbContext.Organizations
                                            .Include(x => x.OrganizationUsers).ThenInclude(x => x.User)
                                            .ToListAsync();

                    request = new ExportRequestModel
                    {
                        FileName = "Organizasyonlar",
                        SheetName = "Organizasyonlar",
                        Title = "Organizasyon Listesi",
                        Columns = new List<ExportColumnModel>
                                    {
                                        new() { Key = "Id", Header = "Id", Width = 10 , DataType = "number"},
                                        new() { Key = "Name", Header = "Ad", Width = 20, DataType = "text" },
                                        new() { Key = "Email", Header = "E-Posta", Width = 30, DataType = "text" },
                                        new() { Key = "Phone", Header = "Telefon", Width = 20, DataType = "text" },
                                        new() { Key = "TaxNo", Header = "Vergi Kimlik Numarası", Width = 20, DataType = "text" },
                                        new() { Key = "TaxOffice", Header = "Vergi Daires", Width = 20, DataType = "text" },
                                        new() { Key = "Users", Header = "Kullanıcılar", Width = 35, DataType = "text" },
                                        new() { Key = "IsDeleted", Header = "Silindi Mi?", Width = 15, DataType = "boolean" },
                                        new() { Key = "IsSystemData", Header = "Sistem Verisi Mi?", Width = 18, DataType = "boolean" }
                                    },
                        Rows = organizations.Select(x => new Dictionary<string, object?>
                        {
                            ["Id"] = x.Id,
                            ["Name"] = x.Name,
                            ["Email"] = x.Email,
                            ["Phone"] = x.Phone,
                            ["TaxNo"] = x.TaxNo,
                            ["TaxOffice"] = x.TaxOffice,
                            ["Users"] = x.OrganizationUsers != null && x.OrganizationUsers.Any() ? string.Join(", ", x.OrganizationUsers.Select(s => s.User.Name + " " + s.User.Surname)) : string.Empty,
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
