using JN_ProyectoWeb.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenter.Servicios
{
    public class General : IGeneral
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public General(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public HttpResponseMessage ObtenerCasosPorUsuario(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Casos/ObtenerCasosPorUsuario";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerCasosPendientes()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Casos/ObtenerTodosLosCasosPendientes";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerCasosDisponibles()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Casos/ObtenerTodosLosCasos";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerCasosAtendido()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "CasoAtendido/ObtenerCasoAtendidoPorId";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerInventario()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Inventario/ObtenerTodoElInventario";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerInventarioDisponible()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Inventario/ObtenerInventarioDisponible";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerArticulos()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Articulo/ObtenerArticulos";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;
            }
        }

        public HttpResponseMessage ObtenerInventarioUtilizadoPorCaso(long casoId)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + $"Inventario/Utilizado/ObtenerPorCaso/{casoId}";
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                return http.GetAsync(url).Result;
            }
        }

        public HttpResponseMessage GestionarInventarioUtilizado(InventarioUtilizadoModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Inventario/GestionarInventarioUtilizado";
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                return http.PostAsJsonAsync(url, model).Result;
            }
        }

        public HttpResponseMessage ActualizarCantidadInventario(long idInventarioUtilizado, int cantidad)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + $"Inventario/ActualizarCantidadInventario/{idInventarioUtilizado}";
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                return http.PutAsJsonAsync(url, cantidad).Result;
            }
        }

        public HttpResponseMessage EliminarInventarioUtilizado(long idInventarioUtilizado)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + $"Inventario/Utilizado/EliminarInventarioUtilizado/{idInventarioUtilizado}";
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                return http.DeleteAsync(url).Result;
            }
        }

        public HttpResponseMessage AgregarInventarioUtilizado(InventarioUtilizadoModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Inventario/AgregarInventarioUtilizado";
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                return http.PostAsJsonAsync(url, model).Result;
            }
        }

        public HttpResponseMessage RestarInventarioUtilizado(InventarioUtilizadoModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Inventario/RestarInventarioUtilizado";
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                return http.PostAsJsonAsync(url, model).Result;
            }
        }


       
    }

}

