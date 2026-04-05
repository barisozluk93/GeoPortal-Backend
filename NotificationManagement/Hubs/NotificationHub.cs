using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace NotificationManagement.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private string? GetCurrentUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? Context.User?.FindFirst("id")?.Value;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"[Hub Connected] Conn={Context.ConnectionId} UserId={userId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetCurrentUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"[Hub Disconnected] Conn={Context.ConnectionId} UserId={userId}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}