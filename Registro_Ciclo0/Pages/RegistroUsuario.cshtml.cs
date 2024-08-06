using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Registro_Ciclo0.Pages
{
    public class RegistroUsuario : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public UserInput Input { get; set; }

        public RegistroUsuario(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await CheckIfExists("/usuarios/check-email?email=" + Input.Correo) ||
                await CheckIfExists("/usuarios/check-username?username=" + Input.Usuario))
            {
                ModelState.AddModelError(string.Empty, "Email or username already exists.");
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            var usuarioData = new
            {
                Nombre = Input.Nombre,
                PrimerApellido = Input.PrimerApellido,
                SegundoApellido = Input.SegundoApellido,
                Direccion = Input.Direccion,
                Telefono = Input.Telefono,
                Usuario = Input.Usuario,
                Correo = Input.Correo,
                Password = Input.Password,
                Genero = Input.Genero,
                FechaNacimiento = Input.FechaNacimiento
            };

            var content = new StringContent(JsonSerializer.Serialize(usuarioData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/usuarios/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return Page();
            }
        }

        private async Task<bool> CheckIfExists(string uri)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ExistenceCheckResult>(responseContent);
                return result.Exists;
            }
            return false;
        }

        private class ExistenceCheckResult
        {
            public bool Exists { get; set; }
        }

        public class UserInput
        {
            public string Nombre { get; set; }
            public string PrimerApellido { get; set; }
            public string SegundoApellido { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Usuario { get; set; }
            public string Correo { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            [Compare("Password", ErrorMessage = "Las contraseñas no son iguales.")]
            public string Confirm_Password { get; set; }
            public string Genero { get; set; }
            public string FechaNacimiento { get; set; }
        }

    }
}