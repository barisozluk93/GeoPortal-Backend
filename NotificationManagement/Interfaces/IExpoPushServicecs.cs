namespace NotificationManagement.Interfaces
{
    public interface IExpoPushService
    {
        Task SendAsync(IEnumerable<string> expoPushTokens, string title, string body, object? data = null);
    }
}