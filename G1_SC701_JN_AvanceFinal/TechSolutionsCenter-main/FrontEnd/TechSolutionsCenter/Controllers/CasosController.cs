

using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TechSolutionsCenter.Models;
using TechSolutionsCenter.Servicios;

namespace TechSolutionsCenter.Controllers
{
    public class CasosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;
        public CasosController(IHttpClientFactory httpClient, IConfiguration configuration, IGeneral general)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _general = general;
        }

        #region RegistrarCaso

        [HttpGet]
        public IActionResult AgregarCaso()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCaso(CasosModel model)
        {
            try
            {
                // Validar modelo
                if (model == null)
                {
                    TempData["Error"] = "Datos del formulario incompletos";
                    return RedirectToAction("ObtenerCasosPendientes", "Casos");
                }

                // Validar imagen (aunque ya se validó en el cliente)
                if (model.ImagenBytes == null || model.ImagenBytes.Length == 0)
                {
                    TempData["Error"] = "La imagen es requerida";
                    return RedirectToAction("ObtenerCasosPendientes", "Casos");
                }

                // Crear modelo para la API
                var casoParaAPI = new CasosModel
                {
                    Titulo = model.Titulo,
                    Descripcion = model.Descripcion,
                    Imagen = model.ImagenBytes,
                    Estado = model.Estado,
                    ID_Usuario = 0,
                    Fecha_Ingreso = DateTime.Now,
                    ImagenBytes = model.ImagenBytes
                };

                using (var http = _httpClient.CreateClient())
                {
                    var url = _configuration.GetSection("Variables:urlWebApi").Value + "Casos/AgregarCaso";
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                    var response = await http.PostAsJsonAsync(url, casoParaAPI);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<RespuestaModel>();
                        TempData["Mensaje"] = result?.Mensaje ?? "Caso registrado correctamente";
                        return RedirectToAction("ObtenerCasosPendientes", "Casos");

                    }

                    TempData["Error"] = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al registrar el caso: {ex.Message}";
            }

            return RedirectToAction("ObtenerCasosPendientes", "Casos");
        }
    


        private async Task<byte[]> ConvertirImagenAVarBinary(IFormFile imagen)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imagen.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


        #endregion

        [HttpGet]
        public IActionResult ObtenerCasosPendientes()
        {
            var response = _general.ObtenerCasosPorUsuario(0);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<CasosModel>>((JsonElement)result.Datos!);
                    return View(datosResult);
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new List<CasosModel>());


            return View();
        }

        [HttpGet]
        public IActionResult ObtenerTodosLosCasosPendientes()
        {
            var response = _general.ObtenerCasosPorUsuario(0);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<CasosModel>>((JsonElement)result.Datos!);
                    return View(datosResult);
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new List<CasosModel>());


            return View();
        }



    }
}
