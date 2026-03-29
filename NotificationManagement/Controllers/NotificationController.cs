using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationManagement.Entity;
using NotificationManagement.Interfaces;
using NotificationManagement.Model;
using System.Security.Claims;

namespace NotificationManagement.Controllers
{
    [Route("/geoPortalApi/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("All/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetAll(long userId)
        {
            var result = await _notificationService.GetNotifications(userId);
            return Ok(result);
        }

        [HttpPost("Save")]
        [AllowAnonymous]
        public async Task<IActionResult> Save([FromBody] Notification notification)
        {
            var result = await _notificationService.Save(notification);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _notificationService.Delete(id);
            return Ok(result);
        }

        [HttpGet("Read/{id}")]
        [Authorize]
        public async Task<IActionResult> Read(long id)
        {
            var result = await _notificationService.Read(id);
            return Ok(result);
        }

        [HttpGet("UnreadCount/{userId}")]
        [Authorize]
        public async Task<IActionResult> UnreadCount(long userId)
        {
            var result = await _notificationService.GetUnreadCount(userId);
            return Ok(result);
        }

        [HttpPost("RegisterDevice")]
        [Authorize]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            var result = await _notificationService.RegisterDevice(request);
            return Ok(result);
        }
    }
}