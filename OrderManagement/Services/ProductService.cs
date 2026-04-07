using Microsoft.EntityFrameworkCore;
using System.Data;
using OrderManagement.Interfaces;
using OrderManagement.DbContexts;
using OrderManagement.Model;
using OrderManagement.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using OrderManagement.Enums;
using System.Text;

namespace OrderManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly OrderManagementContext _dbContext;

        private readonly IConfiguration _configuration;



        public ProductService(OrderManagementContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<PagingResult<PagedList<Product>>>> Paginate(PagingParameter pagingParameter)
        {
            var result = new Result<PagingResult<PagedList<Product>>>();

            string lowerFilterText = string.IsNullOrEmpty(pagingParameter.FilterText) ? null : pagingParameter.FilterText.ToLower();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = _dbContext.Products
                                .Where(x =>
                                    x.Id > 1 &&
                                    (pagingParameter.MaxCloudRate <= -1 ||
                                     (x.CloudRate.HasValue && x.CloudRate.Value <= pagingParameter.MaxCloudRate.Value)) &&
                                    (pagingParameter.MaxPrice <= -1 ||
                                     (x.Price <= pagingParameter.MaxPrice.Value)) &&
                                    (
                                        string.IsNullOrEmpty(lowerFilterText) ||
                                        (x.Name ?? "").ToLower().Contains(lowerFilterText) ||
                                        (x.City ?? "").ToLower().Contains(lowerFilterText) ||
                                        (x.District ?? "").ToLower().Contains(lowerFilterText) ||
                                        (x.Provider ?? "").ToLower().Contains(lowerFilterText)
                                    )
                                )
                                .Select(x => new Product
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    DownloadLink = x.DownloadLink,
                                    Price = x.Price,
                                    PriceStr = x.PriceStr,
                                    IsDeleted = x.IsDeleted,
                                    CategoryId = x.CategoryId,
                                    City = x.City,
                                    District = x.District,
                                    AcquisitionDate = x.AcquisitionDate,
                                    Provider = x.Provider,
                                    Resolution = x.Resolution,
                                    CloudRate = x.CloudRate,
                                    AreaKm2 = x.AreaKm2,
                                    Currency = x.Currency,
                                    ThumbnailUrl = x.ThumbnailUrl,
                                    PreviewUrl = x.PreviewUrl,
                                    Description = x.Description,
                                    IsOrthorectified = x.IsOrthorectified,
                                    IsPansharpened = x.IsPansharpened,
                                    IsClassified = x.IsClassified,
                                    Classes = x.Classes
                                });

                    var pagination = PagedList<Product>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<Product>>()
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

        public async Task<Result<Product>> GetById(long id)
        {
            var result = new Result<Product>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var product = await _dbContext.Products.Include(x => x.Classes).Where(x => x.Id == id).FirstOrDefaultAsync();

                    result.SetData(product);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
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

        
    }
}
