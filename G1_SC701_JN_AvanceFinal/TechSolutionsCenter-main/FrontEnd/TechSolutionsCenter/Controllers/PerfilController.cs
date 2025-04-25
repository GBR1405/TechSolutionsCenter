using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechSolutionsCenter.Models;

namespace TechSolutionsCenter.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;

        public PerfilController(IHttpClientFactory httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Acción para obtener el perfil de usuario
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario") ?? "0");

            if (idUsuario <= 0)
            {
                ModelState.AddModelError("", "Usuario no autenticado.");
                return RedirectToAction("IniciarSesion", "Login");
            }

            using (var http = _httpClient.CreateClient())
            {
                var url = $"{_configuration.GetValue<string>("Variables:urlWebApi")}Perfil/ObtenerPerfil/{idUsuario}";
                try
                {
                    var response = await http.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                        return View(usuario);
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo obtener el perfil.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", $"Error de comunicación: {ex.Message}");
                }
            }

            return View(new UsuarioModel()); // Devolver un modelo vacío si hay error
        }

        // Acción para actualizar el perfil
        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(UsuarioModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Perfil", model); // Si el modelo no es válido, retornar la vista con el modelo actual.
            }

            using (var http = _httpClient.CreateClient())
            {
                var url = $"{_configuration.GetValue<string>("Variables:urlWebApi")}Perfil/ActualizarPerfil";
                try
                {
                    var response = await http.PutAsJsonAsync(url, model);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<RespuestaModel>();

                        if (result != null && result.Indicador)
                        {
                            HttpContext.Session.SetString("NombreUsuario", model.Nombre_Usuario ?? "");
                            HttpContext.Session.SetString("Email", model.Email ?? "");
                            HttpContext.Session.SetString("IdGenero", model.ID_Genero.ToString() ?? "");
                            return RedirectToAction("Perfil");
                        }
                        else
                        {
                            ModelState.AddModelError("", result?.Mensaje ?? "Error desconocido.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al actualizar el perfil.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", $"Error de comunicación: {ex.Message}");
                }
            }

            return View("Perfil", model); // Retornar la vista con el modelo para mostrar los errores
        }

        // Acción para eliminar el perfil
        [HttpPost]
        public async Task<IActionResult> EliminarPerfil()
        {
            var idUsuario = long.Parse(HttpContext.Session.GetString("IdUsuario") ?? "0");

            if (idUsuario <= 0)
            {
                ModelState.AddModelError("", "Usuario no autenticado.");
                return RedirectToAction("IniciarSesion", "Login");
            }

            using (var http = _httpClient.CreateClient())
            {
                var url = $"{_configuration.GetValue<string>("Variables:urlWebApi")}Perfil/EliminarPerfil/{idUsuario}";
                try
                {
                    var response = await http.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContext.Session.Clear();
                        return RedirectToAction("IniciarSesion", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al eliminar el perfil.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", $"Error de comunicación: {ex.Message}");
                }
            }

            return RedirectToAction("Perfil"); // Si hay error, regresar al perfil
        }
    }
}