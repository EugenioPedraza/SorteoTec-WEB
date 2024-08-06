using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Registro_Ciclo0
{
    public class PremiosModel : PageModel
    {

        public string TransactionDetailsMessage { get; set; }


        private readonly IHttpClientFactory _httpClientFactory;

        public PremiosModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Prize { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["LoginRequiredMessage"] = "Ingresa a tu cuenta para hacer uso de la tienda.";
                return RedirectToPage("/Login");
            }

            int articleId = Prize switch
            {
                "XboxSeriesX" => 27,
                "Playstation5" => 28,
                "XboxLiveGold" => 29,
                "PlaystationPlus" => 30,
                "2X1Dinero" => 31,
                "DescuentoAventurat" => 32,
                _ => 0
            };

            if (articleId == 0)
            {
                TempData["Message"] = "Unknown prize selected.";
                return Page();
            }


            var transactionDetails = await GetUserGemsAndArticlePriceAsync(userId, articleId);
            int totalGems = transactionDetails.Item1;
            decimal price = transactionDetails.Item2;

            if (totalGems >= price)
            {

                bool transactionResult = await ExecuteTransactionAsync(int.Parse(userId), articleId, "Compra de premio de mundo real por gemas mediante sitio web.");

                if (transactionResult)
                {
                    int remainingGems = totalGems - (int)price;
                    TransactionDetailsMessage = "¡Transacción completada! Revisa tu correo para mas detalles.";
                }
                else
                {
                    TransactionDetailsMessage = "La transacción falló, por favor intenta de nuevo.";
                }
            }
            else
            {
                TransactionDetailsMessage = "Gemas insuficientes para realizar la compra.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAwardGemsAsync(int gemAmount)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User is not logged in.";
                return RedirectToPage("/Login");
            }

            var client = _httpClientFactory.CreateClient("API");
            var transactionData = new
            {
                userId = int.Parse(userId),
                amount = gemAmount
            };

            var content = new StringContent(JsonSerializer.Serialize(transactionData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/transacciones/regalarGemas", content);

            if (response.IsSuccessStatusCode)
            {
                TransactionDetailsMessage = "Gemas regaladas exitosamente!";
            }
            else
            {
                TransactionDetailsMessage = "La transacción falló, por favor intenta de nuevo.";
            }

            return Page();
        }


        private async Task<Tuple<int, decimal>> GetUserGemsAndArticlePriceAsync(string userId, int articleId)
        {
            var httpClient = _httpClientFactory.CreateClient("API");

            var gemsResponse = await httpClient.GetAsync($"/usuarios/{userId}/gems");
            gemsResponse.EnsureSuccessStatusCode();
            var gemsContent = await gemsResponse.Content.ReadAsStringAsync();
            var gemsData = JsonSerializer.Deserialize<GemsResponse>(gemsContent);

            var priceResponse = await httpClient.GetAsync($"/articulos/{articleId}/price");
            priceResponse.EnsureSuccessStatusCode();
            var priceContent = await priceResponse.Content.ReadAsStringAsync();
            var priceData = JsonSerializer.Deserialize<PriceResponse>(priceContent);

            int totalGems = int.Parse(gemsData.Total_Gemas);
            decimal price = decimal.Parse(priceData.PrecioArticulo);

            return new Tuple<int, decimal>(totalGems, price);
        }

        private class GemsResponse
        {
            public string Total_Gemas { get; set; }
        }

        private class PriceResponse
        {
            public string PrecioArticulo { get; set; }
        }


        private async Task<bool> ExecuteTransactionAsync(int userId, int articleId, string detalle)
        {
            var httpClient = _httpClientFactory.CreateClient("API");

            var transactionData = new
            {
                idUsuario = userId,
                idArticulo = articleId,
                detalle = detalle
            };

            var json = JsonSerializer.Serialize(transactionData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/transacciones/compra", content);

            return response.IsSuccessStatusCode;
        }


    }
}
