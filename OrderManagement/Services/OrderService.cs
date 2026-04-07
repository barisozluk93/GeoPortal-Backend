using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderManagement.DbContexts;
using OrderManagement.Entity;
using OrderManagement.Enums;
using OrderManagement.Interfaces;
using OrderManagement.Model;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderManagementContext _dbContext;

        private readonly IConfiguration _configuration;

        private readonly IExportGateway _exportGateway;

        public OrderService(OrderManagementContext dbContext, IConfiguration configuration, IExportGateway exportGateway)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _exportGateway = exportGateway;
        }

        public async Task<byte[]> ExportExcel(string token)
        {
            var request = await BuildOrderExportRequest(token);
            return await _exportGateway.ExportExcelAsync(request, token);
        }

        public async Task<Result<PagingResult<PagedList<Order>>>> Paginate(PagingParameter pagingParameter, long userId, string? orderNoFiler, double? priceMinFilter, double? priceMaxFilter, string? orderDateFromFilter, string? orderDateToFilter, long? orderStatusStrFilter)
        {
            var result = new Result<PagingResult<PagedList<Order>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    DateTime? orderDateFrom = null;
                    DateTime? orderDateTo = null;

                    if (!string.IsNullOrWhiteSpace(orderDateFromFilter) &&
                        DateTime.TryParse(orderDateFromFilter, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFrom))
                    {
                        orderDateFrom = parsedFrom;
                    }

                    if (!string.IsNullOrWhiteSpace(orderDateToFilter) &&
                        DateTime.TryParse(orderDateToFilter, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo))
                    {
                        orderDateTo = parsedTo;
                    }

                    var queryable = _dbContext.Orders.Include(x => x.Basket)
                                                .Where(x => x.UserId == userId &&
                                                (!String.IsNullOrEmpty(orderNoFiler) ? x.OrderNo.ToLower().Contains(orderNoFiler.ToLower()) : true) &&
                                                            (priceMaxFilter.HasValue ? x.Price <= priceMaxFilter : true) && (priceMinFilter.HasValue ? x.Price >= priceMinFilter : true) &&
                                                            (orderDateFrom.HasValue ? x.OrderDate >= orderDateFrom.Value : true) && (orderDateTo.HasValue ? x.OrderDate <= orderDateTo.Value : true) &&
                                                            (orderStatusStrFilter.HasValue ? x.OrderStatus == orderStatusStrFilter : true))
                                                .Select(s => new Order
                                                {
                                                    Id = s.Id,
                                                    Price = s.Price,
                                                    BasketId = s.BasketId,
                                                    Basket = s.Basket,
                                                    UserId = s.UserId,
                                                    OrderDate = s.OrderDate,
                                                    OrderNo = s.OrderNo,
                                                    OrderStatus = s.OrderStatus,
                                                });

                    var pagination = PagedList<Order>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<Order>>()
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

        public async Task<Result<PagingResult<PagedList<OrderProduct>>>> ComingPaginate(PagingParameter pagingParameter, string token, string? orderNoFiler, decimal? priceMinFilter, decimal? priceMaxFilter, string? orderDateFromFilter, string? orderDateToFilter, long? orderStatusStrFilter)
        {
            var result = new Result<PagingResult<PagedList<OrderProduct>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    DateTime? orderDateFrom = null;
                    DateTime? orderDateTo = null;

                    if (!string.IsNullOrWhiteSpace(orderDateFromFilter) &&
                        DateTime.TryParse(orderDateFromFilter, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFrom))
                    {
                        orderDateFrom = parsedFrom;
                    }

                    if (!string.IsNullOrWhiteSpace(orderDateToFilter) &&
                        DateTime.TryParse(orderDateToFilter, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo))
                    {
                        orderDateTo = parsedTo;
                    }

                    var queryable = _dbContext.OrderProducts.Include(x => x.Order).Include(x => x.Product).Include(x => x.Product)
                        .Where(x => (!String.IsNullOrEmpty(orderNoFiler) ? x.Order.OrderNo.ToLower().Contains(orderNoFiler.ToLower()) : true) &&
                                    (priceMaxFilter.HasValue ? x.Product.Price <= priceMaxFilter : true) && (priceMinFilter.HasValue ? x.Product.Price >= priceMinFilter : true) &&
                                    (orderDateFrom.HasValue ? x.Order.OrderDate >= orderDateFrom.Value : true) && (orderDateTo.HasValue ? x.Order.OrderDate <= orderDateTo.Value : true) &&
                                    (orderStatusStrFilter.HasValue ? x.OrderStatus == orderStatusStrFilter : true))
                        .Select(s => new OrderProduct
                        {
                            Id = s.Id,
                            ProccessDate = s.ProccessDate,
                            Order = s.Order,
                            OrderStatus = s.OrderStatus,
                            ProductId = s.ProductId,
                            Product = s.Product
                        });

                    var pagination = PagedList<OrderProduct>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);
                    result.SetData(new PagingResult<PagedList<OrderProduct>>()
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

        public async Task<Result<List<Order>>> GetOrderList(long userId)
        {
            var result = new Result<List<Order>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var data = await _dbContext.Orders.Where(x => x.UserId == userId).ToListAsync();

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

        public async Task<Result<OrderProduct>> UpdateStatus(OrderProduct orderProduct, string token)
        {
            var result = new Result<OrderProduct>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    string notificationMessage = "";
                    string notificationType = "";

                    var item = await _dbContext.OrderProducts.Include(x => x.Product).Where(x => x.Id == orderProduct.Id).FirstOrDefaultAsync();
                    var order = await _dbContext.Orders.Where(x => x.Id == orderProduct.OrderId).FirstOrDefaultAsync();

                    if (item != null)
                    {
                        if(orderProduct.OrderStatus == 4)
                        {
                            item.CompletionDate = DateTime.UtcNow;
                            notificationType = "ORDER_COMPLETED";
                        }
                        else if (orderProduct.OrderStatus == 3)
                        {
                            notificationType = "ORDER_PREPARING";
                        }
                        else if(orderProduct.OrderStatus == 2)
                        {
                            notificationType = "ORDER_REJECTED";
                        }
                        else if (orderProduct.OrderStatus == 1)
                        {
                            notificationType = "ORDER_APPROVED";
                        }

                        item.ProccessDate = DateTime.UtcNow;
                        item.OrderStatus = orderProduct.OrderStatus;
                        await _dbContext.SaveChangesAsync();

                        if (item.OrderStatus == 4)
                        {
                            if(!(_dbContext.OrderProducts.Where(x => x.OrderId == orderProduct.OrderId && x.OrderStatus != (int)OrderStatus.Tamamlandi).Any()))
                            {
                                order.OrderStatus = (int)OrderStatus.SiparisTamamlandi;
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        else if (item.OrderStatus == 2)
                        {
                            order.OrderStatus = (int)OrderStatus.SiparisTamamlandi;
                            await _dbContext.SaveChangesAsync();
                        }

                        Notification notification = new Notification();
                        notification.Type = notificationType;
                        notification.Title = "GeoPortal";
                        notification.Body = item.Order.OrderNo + "/" + item.Product.Name;
                        notification.IsDeleted = false;
                        notification.IsRead = false;
                        notification.UserId = order.UserId;
                        notification.CreatedAt = DateTime.UtcNow;
                        notification.TargetUrl = "" + order.Id;

                        await SaveNotification(notification, token);

                        transaction.Commit();

                        result.SetData(item);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }

                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(exception.Message);
                }
            }

            return result;
        }
        public async Task<Result<Order>> Save(Order order, string token)
        {
            var result = new Result<Order>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var superUsers = await GetSuperUsers(token);

                    order.OrderDate = DateTime.UtcNow;
                    order.OrderStatus = (int)OrderStatus.SiparisTamamlanmadi;
                    order.OrderNo = Guid.NewGuid().ToString();
                    _dbContext.Add(order);
                    await _dbContext.SaveChangesAsync();

                    var basket = await _dbContext.Baskets.Where(x => x.Id == order.BasketId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    basket.IsActive = false;

                    var basketProducts = await _dbContext.BasketProducts.Include(x => x.Product).Where(x => x.BasketId == order.BasketId && x.IsActive && !x.IsDeleted).ToListAsync();
                    basketProducts.ForEach(x => x.IsActive = false);
                    await _dbContext.SaveChangesAsync();

                    foreach (var basketProduct in basketProducts)
                    {
                        OrderProduct orderProduct = new OrderProduct();
                        orderProduct.OrderId = order.Id;
                        orderProduct.ProductId = basketProduct.ProductId;
                        orderProduct.ProccessDate = order.OrderDate.Value;
                        orderProduct.Product = basketProduct.Product;

                        if (basketProduct.Product.CategoryId == 2)
                        {
                            orderProduct.OrderStatus = (int)OrderStatus.Tamamlandi;

                            if (basketProducts.Count() == 1)
                            {
                                order.OrderStatus = (int)OrderStatus.SiparisTamamlandi;
                            }
                        }
                        else
                        {
                            orderProduct.OrderStatus = (int)OrderStatus.OnayBekliyor;
                        }

                        _dbContext.Add(orderProduct);
                        await _dbContext.SaveChangesAsync();

                        if (basketProduct.Product.CategoryId == 2)
                        {
                            var generatedApiKey = await CreateApiKeyAsync(order, orderProduct);
                            orderProduct.ProductValue = generatedApiKey;
                            await _dbContext.SaveChangesAsync();
                        }

                        if(order.OrderStatus == (int)OrderStatus.SiparisTamamlandi)
                        {
                            Notification notification = new Notification();
                            notification.Type = "ORDER_COMPLETED";
                            notification.Title = "GeoPortal";
                            notification.Body = order.OrderNo + "/" + orderProduct.Product.Name;
                            notification.IsDeleted = false;
                            notification.IsRead = false;
                            notification.UserId = order.UserId;
                            notification.CreatedAt = DateTime.UtcNow;
                            notification.TargetUrl = "" + order.Id;

                            await SaveNotification(notification, token);
                        }

                        foreach (var superUser in superUsers)
                        {
                            Notification notification = new Notification();
                            notification.Type = "NEW_ORDER";
                            notification.Title = "GeoPortal";
                            notification.Body = order.OrderNo + "/" + orderProduct.Product.Name;
                            notification.IsDeleted = false;
                            notification.IsRead = false;
                            notification.UserId = superUser;
                            notification.CreatedAt = DateTime.UtcNow;
                            notification.TargetUrl = "" + orderProduct.Id;

                            await SaveNotification(notification, token);
                        }
                    }


                    transaction.Commit();

                    result.SetData(order);
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

        public async Task<Result<Order>> GetById(long id, string token)
        {
            var result = new Result<Order>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var order = await _dbContext.Orders.Include(x => x.Basket).Where(x => x.Id == id).FirstOrDefaultAsync();

                    order.OrderProducts = await _dbContext.OrderProducts
                                                    .Include(x => x.Order)
                                                    .Include(x => x.Product)
                                                    .Where(x => x.OrderId == id).ToListAsync();


                    foreach (var orderProduct in order.OrderProducts)
                    {
                        if(orderProduct.FileId.HasValue)
                        {
                            orderProduct.FileName = GetFile(orderProduct.FileId.Value, token).Result.Key;
                            orderProduct.FileResult = GetFile(orderProduct.FileId.Value, token).Result.Value;
                        }
                    }

                    result.SetData(order);
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

        public async Task<Result<OrderProduct>> GetComingOrderById(long id, string token)
        {
            var result = new Result<OrderProduct>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var orderProduct = await _dbContext.OrderProducts.Include(x => x.Order).Include(x => x.Product).Where(x => x.Id == id).FirstOrDefaultAsync();

                    orderProduct.Order.InvoiceAddress = await GetAddress(orderProduct.Order.InvoiceAddressId, token);

                    if (orderProduct.FileId.HasValue)
                    {
                        orderProduct.FileName = GetFile(orderProduct.FileId.Value, token).Result.Key;
                        orderProduct.FileResult = GetFile(orderProduct.FileId.Value, token).Result.Value;
                    }

                    result.SetData(orderProduct);
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

        public async Task<Result<OrderProduct>> AddInvoice(OrderProduct orderProduct, string token)
        {
            var result = new Result<OrderProduct>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var item = await _dbContext.OrderProducts.Include(x => x.Product).Where(x => x.Id == orderProduct.Id).FirstOrDefaultAsync();

                    if (item != null)
                    {
                        if (item.FileId.HasValue)
                        {
                            var fileDeleteResult = DeleteFile(item.FileId.Value, token).Result;
                        }

                        item.FileId = orderProduct.FileId;

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(item);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
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

        public async Task<Result<OrderProduct>> DeleteInvoice(long id, string token)
        {
            var result = new Result<OrderProduct>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var item = await _dbContext.OrderProducts.Include(x => x.Order).Include(x => x.Product).Where(x => x.Id == id).FirstOrDefaultAsync();

                    if (item != null)
                    {
                        var fileDeleteResult = DeleteFile(item.FileId.Value, token).Result;

                        if (fileDeleteResult.Key)
                        {
                            item.FileId = null;
                            item.FileName = null;
                            item.FileResult = null;

                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();

                            result.SetData(item);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage(fileDeleteResult.Value);
                        }
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

        private async Task<bool> SaveNotification(Notification notification, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var serializedNotification = JsonConvert.SerializeObject(notification);
            StringContent stringContent = new StringContent(serializedNotification, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_configuration["AppSettings:ApiUrl"] + "/geoPortalApi/Notification/Save", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Notification> result = JsonConvert.DeserializeObject<Result<Notification>>(responseStr);

                        if (result.GetIsSuccess().Value)
                        {
                            return result.GetIsSuccess().Value;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return false;
        }

        private async Task<KeyValuePair<bool, string?>> DeleteFile(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync(_configuration["AppSettings:ApiUrl"] + "/geoPortalApi/File/Delete/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Model.File> result = JsonConvert.DeserializeObject<Result<Model.File>>(responseStr);

                        if (result != null)
                        {
                            byte[] bytes = System.IO.File.ReadAllBytes(result.GetData().Path);
                            return new KeyValuePair<bool, string?>(result.GetIsSuccess().Value, result.GetMessage());
                        }
                        else
                        {
                            return new KeyValuePair<bool, string?>(false, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new KeyValuePair<bool, string?>(false, null);
                    }

                }
                else
                {
                    return new KeyValuePair<bool, string?>(false, null);
                }
            }
            else
            {
                return new KeyValuePair<bool, string?>(false, null);
            }

            return new KeyValuePair<bool, string?>(false, null);
        }

        private async Task<KeyValuePair<string, FileContentResult>> GetFile(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/geoPortalApi/File/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Model.File> result = JsonConvert.DeserializeObject<Result<Model.File>>(responseStr);

                        if (result != null)
                        {
                            byte[] bytes = System.IO.File.ReadAllBytes(result.GetData().Path);
                            return new KeyValuePair<string, FileContentResult>(result.GetData().Name, new FileContentResult(bytes, result.GetData().ContentType));
                        }
                        else
                        {
                            return new KeyValuePair<string, FileContentResult>(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new KeyValuePair<string, FileContentResult>(null, null);
                    }

                }
                else
                {
                    return new KeyValuePair<string, FileContentResult>(null, null);
                }
            }
            else
            {
                return new KeyValuePair<string, FileContentResult>(null, null);
            }

            return new KeyValuePair<string, FileContentResult>(null, null);
        }

        private async Task<string> GetUserName(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/geoPortalApi/User/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<User> result = JsonConvert.DeserializeObject<Result<User>>(responseStr);
                        string userName = result.GetData().Name + " " + result.GetData().Surname;

                        return userName;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }

                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        private async Task<UserAddress> GetAddress(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/geoPortalApi/User/UserAddressById/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<UserAddress> result = JsonConvert.DeserializeObject<Result<UserAddress>>(responseStr);

                        return result.GetData();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private async Task<List<long>> GetSuperUsers(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/geoPortalApi/User/GetSuperUserList");

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<List<long>> result = JsonConvert.DeserializeObject<Result<List<long>>>(responseStr);

                        return result.GetData();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task<Result<ValidateApiKeyResponse>> ValidateApiKey(ValidateApiKeyRequest request)
        {
            var result = new Result<ValidateApiKeyResponse>();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

            try
            {
                if (request == null)
                {
                    await transaction.RollbackAsync();

                    result.SetIsSuccess(false);
                    result.SetMessage("Request boş olamaz.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        Error = "Request boş olamaz."
                    });
                    return result;
                }

                if (string.IsNullOrWhiteSpace(request.ApiKey))
                {
                    await transaction.RollbackAsync();

                    result.SetIsSuccess(false);
                    result.SetMessage("ApiKey zorunludur.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        Error = "ApiKey zorunludur."
                    });
                    return result;
                }

                if (string.IsNullOrWhiteSpace(request.Endpoint))
                {
                    await transaction.RollbackAsync();

                    result.SetIsSuccess(false);
                    result.SetMessage("Endpoint zorunludur.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        Error = "Endpoint zorunludur."
                    });
                    return result;
                }

                var normalizedApiKey = request.ApiKey.Trim();
                var normalizedEndpoint = request.Endpoint.Trim().ToLower();

                var apiKey = await _dbContext.ApiKeys
                    .Include(x => x.Permissions)
                    .FirstOrDefaultAsync(x => x.Key == normalizedApiKey);

                if (apiKey == null)
                {
                    await transaction.RollbackAsync();

                    result.SetIsSuccess(false);
                    result.SetMessage("Api key bulunamadı.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        Error = "Invalid key"
                    });
                    return result;
                }

                if (!apiKey.IsActive)
                {
                    await transaction.RollbackAsync();

                    result.SetIsSuccess(false);
                    result.SetMessage("Api key pasif.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        ApiKeyId = apiKey.Id,
                        UserId = apiKey.UserId,
                        OrderId = apiKey.OrderId,
                        OrderProductId = apiKey.OrderProductId,
                        Error = "Inactive key"
                    });
                    return result;
                }

                if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value < DateTime.UtcNow)
                {
                    apiKey.IsActive = false;
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetIsSuccess(false);
                    result.SetMessage("Api key süresi dolmuş.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        ApiKeyId = apiKey.Id,
                        UserId = apiKey.UserId,
                        OrderId = apiKey.OrderId,
                        OrderProductId = apiKey.OrderProductId,
                        Error = "Expired key"
                    });
                    return result;
                }

                var hasPermission = apiKey.Permissions != null && apiKey.Permissions.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Endpoint) &&
                    normalizedEndpoint.StartsWith(x.Endpoint.Trim().ToLower()));

                if (!hasPermission)
                {
                    await transaction.RollbackAsync();

                    result.SetIsSuccess(false);
                    result.SetMessage("Bu endpoint için yetki yok.");
                    result.SetData(new ValidateApiKeyResponse
                    {
                        IsValid = false,
                        ApiKeyId = apiKey.Id,
                        UserId = apiKey.UserId,
                        OrderId = apiKey.OrderId,
                        OrderProductId = apiKey.OrderProductId,
                        Error = "Permission denied"
                    });
                    return result;
                }

                result.SetData(new ValidateApiKeyResponse
                {
                    IsValid = true,
                    ApiKeyId = apiKey.Id,
                    UserId = apiKey.UserId,
                    OrderId = apiKey.OrderId,
                    OrderProductId = apiKey.OrderProductId
                });
                result.SetMessage("Api key doğrulandı.");

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
                result.SetData(new ValidateApiKeyResponse
                {
                    IsValid = false,
                    Error = ex.Message
                });

                return result;
            }
        }
        private async Task<string> CreateApiKeyAsync(Order order, OrderProduct orderProduct)
        {
            var apiKeyValue = GenerateApiKeyValue();
            var permissionEndpoints = _configuration
                .GetSection("ApiKeySettings:AllowedEndpoints")
                .Get<string[]>()
                ?.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            if (permissionEndpoints.Count == 0)
            {
                throw new InvalidOperationException("ApiKeySettings:AllowedEndpoints alanı boş olamaz.");
            }

            var expireMonths = _configuration.GetValue<int?>("ApiKeySettings:ExpireMonths") ?? 1;

            var apiKey = new ApiKey
            {
                Key = apiKeyValue,
                UserId = order.UserId,
                OrderId = order.Id,
                OrderProductId = orderProduct.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(expireMonths),
                Permissions = permissionEndpoints
                    .Select(endpoint => new ApiKeyPermission
                    {
                        Endpoint = endpoint
                    })
                    .ToList()
            };

            _dbContext.ApiKeys.Add(apiKey);
            await _dbContext.SaveChangesAsync();

            return apiKeyValue;
        }

        private static string GenerateApiKeyValue()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes)
                .Replace("+", string.Empty)
                .Replace("/", string.Empty)
                .Replace("=", string.Empty);
        }

        private async Task<ExportRequestModel> BuildOrderExportRequest(string token)
        {
            ExportRequestModel request = null;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var orders = await _dbContext.Orders
                                            .Select(s => new Order
                                            {
                                                Id = s.Id,
                                                Price = s.Price,
                                                BasketId = s.BasketId,
                                                Basket = s.Basket,
                                                UserId = s.UserId,
                                                OrderDate = s.OrderDate,
                                                OrderNo = s.OrderNo,
                                                OrderStatus = s.OrderStatus,
                                                OrderProducts = _dbContext.OrderProducts.Include(x => x.Product).Where(x => x.OrderId == s.Id).ToList()
                                            }).ToListAsync();

                    orders.ForEach(x => x.InvoiceAddress = GetAddress(x.UserId, token).Result);

                    request = new ExportRequestModel
                    {
                        FileName = "Siparişler",
                        SheetName = "Siparişler",
                        Title = "Sipariş Listesi",

                        Columns = new List<ExportColumnModel>
                        {
                            new() { Key = "Id", Header = "Id", Width = 10, DataType = "number" },
                            new() { Key = "OrderNo", Header = "Sipariş No", Width = 25, DataType = "text" },
                            new() { Key = "Price", Header = "Fiyat", Width = 15, DataType = "number", Format = "#,##0.00" },
                            new() { Key = "OrderDate", Header = "Sipariş Tarihi", Width = 22, DataType = "datetime" },
                            new() { Key = "OrderStatus", Header = "Durum", Width = 20, DataType = "text" },
                            new() { Key = "Customer", Header = "Kullanıcı Id", Width = 15, DataType = "number" },
                            new() { Key = "ProductCount", Header = "Ürün Sayısı", Width = 15, DataType = "number" },
                            new() { Key = "Products", Header = "Ürünler", Width = 40, DataType = "text" },
                            new() { Key = "InvoiceAddress", Header = "Adres", Width = 40, DataType = "text" }
                        },
                        Rows = orders.Select(x => new Dictionary<string, object?>
                        {
                            ["Id"] = x.Id,
                            ["OrderNo"] = x.OrderNo,
                            ["Price"] = x.Price,
                            ["OrderDate"] = x.OrderDate,
                            ["OrderStatus"] = ((OrderStatus)x.OrderStatus).ToString(),
                            ["Customer"] = GetUserName(x.UserId, token).Result,
                            ["ProductCount"] = x.OrderProducts?.Count ?? 0,
                            ["Products"] = x.OrderProducts != null && x.OrderProducts.Any()
                                ? string.Join(", ", x.OrderProducts.Select(p => p.Product.Name))
                                : string.Empty,
                            ["InvoiceAddress"] = x.InvoiceAddress != null
                                ? $"{x.InvoiceAddress.Address} / {x.InvoiceAddress.City}"
                                : string.Empty

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

