using System.Net.Http.Json;
using NotificationManagement.Interfaces;

namespace NotificationManagement.Services
{
    public class ExpoPushService : IExpoPushService
    {
        private readonly HttpClient _httpClient;

        public ExpoPushService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendAsync(IEnumerable<string> expoPushTokens, string title, string body, object? data = null)
        {
            var tokens = expoPushTokens
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (!tokens.Any())
                return;

            var messages = tokens.Select(token => new
            {
                to = token,
                sound = "default",
                title,
                body,
                data
            }).ToList();

            var response = await _httpClient.PostAsJsonAsync(
                "https://exp.host/--/api/v2/push/send",
                messages
            );

            var responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine("=== EXPO PUSH RESPONSE ===");
            Console.WriteLine(responseText);

            response.EnsureSuccessStatusCode();
        }
    }
}