using UserManagement.Entity;
using UserManagement.Model;

namespace UserManagement.Interfaces
{
    public interface IOrganizationService
    {
        Task<Result<PagingResult<PagedList<Organization>>>> Paginate(PagingParameter pagingParameter, bool? isDeletedFilter, string? nameFilter, string? taxNoFilter, string? taxOfficeFilter, string? emailFilter, string? phoneFilter);
        Task<Result<List<Organization>>> GetOrganizations();
        Task<Result<Organization>> Save(Organization organization);
        Task<Result<Organization>> Update(Organization organization);
        Task<Result<Organization>> Delete(long id);
        Task<Result<Organization>> GetById(long id);
        Task<byte[]> ExportExcel(string token);

    }
}
