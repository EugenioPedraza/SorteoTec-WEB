using System;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Registro_Ciclo0.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory clientFactory, ILogger<LoginModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            _logger.LogInformation("Login attempt with Email: {Email}", Email);

            var client = _clientFactory.CreateClient("API");
            var loginData = new
            {
                Correo = Email,
                Password = Password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("/usuarios/login", content);

            if (loginResponse.IsSuccessStatusCode)
            {
                
                var userStatusResponse = await client.GetAsync($"/usuarios/adminstatus?email={Uri.EscapeDataString(Email)}");
                if (userStatusResponse.IsSuccessStatusCode)
                {
                    var userStatusContent = await userStatusResponse.Content.ReadAsStringAsync();
                    var userStatusResult = JsonSerializer.Deserialize<UserStatusResult>(userStatusContent);

                   
                    HttpContext.Session.SetString("UserId", userStatusResult.IdUsuario.ToString());
                    HttpContext.Session.SetString("IsAdmin", userStatusResult.Administrador.ToString());
                    var isAdminString = HttpContext.Session.GetString("IsAdmin");
                    bool isAdmin = isAdminString == "1";

                    if (isAdmin)
                    {
                        return RedirectToPage("/Admin");
                    }

                    return RedirectToPage("Index");
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve user status for: {Email}", Email);
                    ModelState.AddModelError(string.Empty, "Failed to retrieve user status.");
                    return Page();
                }
            }
            else
            {
                _logger.LogWarning("Contraseña incorrecta para: {Email}", Email);
                ModelState.AddModelError(string.Empty, "Información incorrecta.");
                return Page();
            }



        }

        public void OnGet()
        {
            if (TempData["LoginRequiredMessage"] is string loginRequiredMessage)
            {
                ModelState.AddModelError(string.Empty, loginRequiredMessage);
            }
        }

        private class UserStatusResult
        {
            public int IdUsuario { get; set; }
            public int Administrador { get; set; }
        }
    }
}