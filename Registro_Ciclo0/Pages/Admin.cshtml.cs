using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Registro_Ciclo0.Pages
{
    public class AdminModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public string GenderDataJson { get; private set; }
        public string AgeDataJson { get; private set; }

        public AdminModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            bool isAdmin = isAdminString == "1";

            if (!isAdmin)
            {
                return RedirectToPage("/Index");
            }

            var client = _clientFactory.CreateClient("API");
            var genderResponse = await client.GetAsync("/usuarios/genders");
            var ageResponse = await client.GetAsync("/usuarios/age-ranges");

            if (genderResponse.IsSuccessStatusCode && ageResponse.IsSuccessStatusCode)
            {
                GenderDataJson = await genderResponse.Content.ReadAsStringAsync();
                AgeDataJson = await ageResponse.Content.ReadAsStringAsync();
                return Page();
            }
            else
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
