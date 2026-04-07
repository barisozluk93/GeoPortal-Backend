using UserManagement.Interfaces;
using UserManagement.Model;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace UserManagement.Services
{
    public class ExportGateway : IExportGateway
    {
        private readonly HttpClient _httpClient;

        public ExportGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> ExportExcelAsync(ExportRequestModel request, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("geoPortalApi/Export/Excel", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> ExportPdfAsync(ExportRequestModel request, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("geoPortalApi/Export/Pdf", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}