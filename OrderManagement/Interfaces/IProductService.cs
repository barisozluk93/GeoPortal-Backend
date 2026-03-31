using OrderManagement.Entity;
using OrderManagement.Model;

namespace OrderManagement.Interfaces
{
    public interface IProductService
    {
        Task<Result<PagingResult<PagedList<Product>>>> Paginate(PagingParameter pagingParameter);
        Task<Result<Product>> GetById(long id);
       
    }
}
