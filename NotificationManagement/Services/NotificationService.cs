using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationManagement.DbContexts;
using NotificationManagement.Entity;
using NotificationManagement.Hubs;
using NotificationManagement.Interfaces;
using NotificationManagement.Model;
using System.Data;

namespace NotificationManagement.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationManagementContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IExpoPushService _expoPushService;

        public NotificationService(
            NotificationManagementContext dbContext,
            IHubContext<NotificationHub> hubContext,
            IExpoPushService expoPushService)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _expoPushService = expoPushService;
        }

        public async Task<Result<List<Notification>>> GetNotifications(long userId)
        {
            var result = new Result<List<Notification>>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var data = await _dbContext.Notifications
                    .Where(x => !x.IsDeleted && x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                result.SetData(data);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<Notification>> Save(Notification request)
        {
            var result = new Result<Notification>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var notification = new Notification
                {
                    UserId = request.UserId,
                    Type = request.Type,
                    Title = request.Title,
                    Body = request.Body,
                    TargetUrl = request.TargetUrl,
                    DataJson = request.DataJson,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _dbContext.Notifications.Add(notification);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                Console.WriteLine("=== NOTIFICATION SAVE ===");
                Console.WriteLine("Notification UserId: " + notification.UserId);

                await _hubContext.Clients
                    .User(notification.UserId.ToString())
                    .SendAsync("notificationReceived", notification);

                var unreadCount = await _dbContext.Notifications
                    .CountAsync(x => x.UserId == notification.UserId && !x.IsDeleted && !x.IsRead);

                await _hubContext.Clients
                    .User(notification.UserId.ToString())
                    .SendAsync("unreadCountChanged", unreadCount);

                var pushTokens = await _dbContext.UserDevices
                    .Where(x => x.UserId == notification.UserId && x.IsActive)
                    .Select(x => x.PushToken)
                    .ToListAsync();

                Console.WriteLine("Push token count: " + pushTokens.Count);

                await _expoPushService.SendAsync(
                    pushTokens,
                    notification.Title,
                    notification.Body,
                    new
                    {
                        notificationId = notification.Id,
                        targetUrl = notification.TargetUrl,
                        type = notification.Type
                    });

                result.SetData(notification);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<Notification>> Delete(long id)
        {
            var result = new Result<Notification>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var oldNotification = await _dbContext.Notifications
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (oldNotification == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    return result;
                }

                oldNotification.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                result.SetData(oldNotification);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<Notification>> Read(long id)
        {
            var result = new Result<Notification>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var notification = await _dbContext.Notifications
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (notification == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    return result;
                }

                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }

                transaction.Commit();

                var unreadCount = await _dbContext.Notifications
                    .CountAsync(x => x.UserId == notification.UserId && !x.IsDeleted && !x.IsRead);

                await _hubContext.Clients
                    .User(notification.UserId.ToString())
                    .SendAsync("unreadCountChanged", unreadCount);

                result.SetData(notification);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<int>> GetUnreadCount(long userId)
        {
            var result = new Result<int>();

            try
            {
                var unreadCount = await _dbContext.Notifications
                    .CountAsync(x => x.UserId == userId && !x.IsDeleted && !x.IsRead);

                result.SetData(unreadCount);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<bool>> RegisterDevice(RegisterDeviceRequest request)
        {
            var result = new Result<bool>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var device = await _dbContext.UserDevices
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.PushToken == request.PushToken);

                if (device == null)
                {
                    device = new UserDevice
                    {
                        UserId = request.UserId,
                        Platform = request.Platform,
                        PushToken = request.PushToken,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastSeenAt = DateTime.UtcNow
                    };

                    _dbContext.UserDevices.Add(device);
                }
                else
                {
                    device.Platform = request.Platform;
                    device.IsActive = true;
                    device.LastSeenAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                result.SetData(true);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }
    }
}