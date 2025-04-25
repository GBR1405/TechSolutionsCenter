using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TechSolutionsCenter.Models;
using TechSolutionsCenter.Servicios;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenter.Controllers
{
    public class InventarioController : Controller
    {

        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;
        public InventarioController(IHttpClientFactory httpClient, IConfiguration configuration, IGeneral general)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _general = general;
        }

        #region Inventario
        [HttpGet]
        public IActionResult ObtenerInventarioTotal()
        {
            var response = _general.ObtenerInventario();

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<InventarioModel>>((JsonElement)result.Datos!);
                    return View(datosResult);
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new List<InventarioModel>());
        }


        #endregion

        #region Articulos

        [HttpGet]
        public IActionResult GestionInventario()
        {
            var responseInventario = _general.ObtenerInventario();
            var responseArticulos = _general.ObtenerArticulos();

            var viewModel = new InventarioArticulosViewModel
            {
                Inventario = new List<InventarioModel>(),
                Articulos = new List<InventarioModel>()
            };

            if (responseInventario.IsSuccessStatusCode)
            {
                var result = responseInventario.Content.ReadFromJsonAsync<RespuestaModel>().Result;
                if (result != null && result.Indicador)
                {
                    viewModel.Inventario = JsonSerializer.Deserialize<List<InventarioModel>>((JsonElement)result.Datos!);
                }
            }

            if (responseArticulos.IsSuccessStatusCode)
            {
                var result = responseArticulos.Content.ReadFromJsonAsync<RespuestaModel>().Result;
                if (result != null && result.Indicador)
                {
                    viewModel.Articulos = JsonSerializer.Deserialize<List<InventarioModel>>((JsonElement)result.Datos!);
                }
            }

            viewModel.NuevoInventario = new InventarioModel();
            viewModel.NuevoArticulo = new InventarioModel();

            return View(viewModel);
        }




        #endregion
    }
}
