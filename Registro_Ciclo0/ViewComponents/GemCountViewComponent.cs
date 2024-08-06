using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Registro_Ciclo0.ViewComponents
{
    public class GemCountViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GemCountViewComponent(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            int? gemCount = null; // Make gemCount nullable

            if (!string.IsNullOrEmpty(userId))
            {
                gemCount = await GetUserGemsAsync(userId);
            }

            return View(gemCount); // Pass the nullable gemCount to the view
        }

        private async Task<int?> GetUserGemsAsync(string userId) // Return nullable int
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.GetAsync($"/usuarios/{userId}/gems");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gemsResponse = JsonSerializer.Deserialize<GemsResponse>(content);
                return int.Parse(gemsResponse?.Total_Gemas ?? "0");
            }

            return null; // Return null if there is an error in fetching gems
        }

        private class GemsResponse
        {
            public string Total_Gemas { get; set; }
        }
    }
}
