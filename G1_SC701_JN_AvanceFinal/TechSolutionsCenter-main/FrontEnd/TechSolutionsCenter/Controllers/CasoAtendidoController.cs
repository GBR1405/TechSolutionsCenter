using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using TechSolutionsCenter.Models;
using TechSolutionsCenter.Servicios;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenter.Controllers
{
    public class CasoAtendidoController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;
        public CasoAtendidoController(IHttpClientFactory httpClient, IConfiguration configuration, IGeneral general)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _general = general;
        }

        [HttpGet]
        public IActionResult ListaCasosPendientes()
        {
            var response = _general.ObtenerCasosPendientes();

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
        }

        [HttpPost]
        public async Task<IActionResult> EquipoEntregado(long id)
        {
            try
            {
                using (var http = _httpClient.CreateClient())
                {
                    var url = _configuration.GetSection("Variables:urlWebApi").Value + "Casos/EditarCasoPendiente";

                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                    // Cambia esta parte para enviar el objeto correctamente
                    var content = new { idCaso = id };
                    var response = await http.PostAsJsonAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<RespuestaModel>();

                        if (result != null && result.Indicador)
                        {
                            return Json(new
                            {
                                success = true,
                                redirectUrl = Url.Action("ListaCasosPendientes", "CasoAtendido")
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                success = false,
                                message = result?.Mensaje ?? "No se pudo completar la operación"
                            });
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            message = $"Error en la comunicación con el servidor: {response.StatusCode} - {errorContent}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult ObtenerCasosAAtender()
        {
            var response = _general.ObtenerCasosDisponibles();

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
        }

        [HttpPost]
        public async Task<IActionResult> AsignarCaso(long id)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "ID de caso no válido"
                    });
                }

                // Crear el objeto de request con todas las propiedades requeridas
                var casoAtendido = new
                {
                    ID_Caso = id,
                    ID_Usuario = 0, // Asegúrate de usar el ID de usuario correcto
                    Fecha_Atencion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), // Formato ISO
                                                                                   // Agrega otras propiedades requeridas por tu API
                    Estado = 'P' // Ejemplo: 'P' para Pendiente o el estado inicial que necesites
                };

                using (var http = _httpClient.CreateClient())
                {
                    var url = _configuration.GetSection("Variables:urlWebApi").Value + "CasoAtendido/AgregarCasoAtendido";
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                    // Usar PostAsJsonAsync directamente con el objeto anónimo
                    var response = await http.PostAsJsonAsync(url, casoAtendido);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<RespuestaModel>();

                        if (result != null && result.Indicador)
                        {
                            return Json(new
                            {
                                success = true,
                                message = "El caso ha sido asignado correctamente",
                                redirectUrl = Url.Action("ListaCasosPendientes", "CasoAtendido")
                            });
                        }
                        return Json(new
                        {
                            success = false,
                            message = result?.Mensaje ?? "No se pudo completar la operación"
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            message = $"Error en el servidor: {errorContent}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        // Clase auxiliar para el request
        public class AsignarCasoRequest
        {
            public long Id { get; set; }
        }

        #region RepararCaso

        [HttpGet]
        public IActionResult ObtenerCasoAntendido()
        {
            var response = _general.ObtenerCasosAtendido();

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<CasoAtendidoModel>((JsonElement)result.Datos!);
                    return View(datosResult);
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new List<CasoAtendidoModel>());
        }

        #endregion

        [HttpGet]
        public IActionResult ObtenerInventarioDisponible()
        {
            try
            {
                var response = _general.ObtenerInventarioDisponible();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error al obtener el inventario");
                }

                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result == null || !result.Indicador)
                {
                    return BadRequest(result?.Mensaje ?? "Error en la respuesta del servicio");
                }

                var inventario = JsonSerializer.Deserialize<List<InventarioDisponibleModel>>((JsonElement)result.Datos!);
                return Ok(inventario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult ObtenerInventarioAsignado(int casoId)
        {
            try
            {
                var response = _general.ObtenerInventarioUtilizadoPorCaso(casoId);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error al obtener el inventario asignado");
                }

                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result == null || !result.Indicador)
                {
                    return BadRequest(result?.Mensaje ?? "Error en la respuesta del servicio");
                }

                var inventarioAsignado = JsonSerializer.Deserialize<List<InventarioAsignadoModel>>((JsonElement)result.Datos!);
                return Ok(inventarioAsignado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult GuardarInventarioAsignado([FromBody] GuardarInventarioModel model)
        {
            try
            {
                // Validar modelo
                if (model == null || model.casoId <= 0)
                {
                    return BadRequest("Datos de solicitud inválidos");
                }

                // Obtener inventario actual asignado
                var currentItemsResponse = ObtenerInventarioAsignado(model.casoId);
                if (currentItemsResponse is not OkObjectResult okResult)
                {
                    return StatusCode(500, "No se pudo verificar el inventario actual");
                }

                var currentItems = okResult.Value as List<InventarioAsignadoModel> ?? new List<InventarioAsignadoModel>();

                // Procesar cada item del modelo
                foreach (var item in model.items)
                {
                    var existingItem = currentItems.FirstOrDefault(x => x.ID_Inventario == item.ID_Inventario);

                    if (existingItem != null)
                    {
                        // Actualizar cantidad si es diferente
                        if (existingItem.Cantidad != item.Cantidad)
                        {
                            var updateResponse = _general.ActualizarCantidadInventario(
                                existingItem.ID_InventarioUtilizado,
                                item.Cantidad);

                            if (!updateResponse.IsSuccessStatusCode)
                            {
                                var errorResult = updateResponse.Content.ReadFromJsonAsync<RespuestaModel>().Result;
                                return BadRequest(errorResult?.Mensaje ?? "Error al actualizar el inventario");
                            }
                        }
                    }
                    else
                    {

                    }
                }

                // Eliminar items que ya no están en la lista
                foreach (var currentItem in currentItems)
                {
                    if (!model.items.Any(x => x.ID_Inventario == currentItem.ID_Inventario))
                    {
                        var deleteResponse = _general.EliminarInventarioUtilizado(currentItem.ID_InventarioUtilizado);
                        if (!deleteResponse.IsSuccessStatusCode)
                        {
                            var errorResult = deleteResponse.Content.ReadFromJsonAsync<RespuestaModel>().Result;
                            return BadRequest(errorResult?.Mensaje ?? "Error al eliminar el inventario");
                        }
                    }
                }

                return Ok(new { success = true, message = "Inventario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarCaso(long Caso, string comentario, char estadoFinal)
        {
            try
            {
                var factura = new FacturaModel
                {
                    ID_CasoAtendido = Caso,
                    Comentario = comentario,
                    Fecha = DateTime.Now
                };

                using (var http = _httpClient.CreateClient())
                {
                    var url = _configuration.GetSection("Variables:urlWebApi").Value + "Factura/GenerarFactura";
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                    var response = await http.PostAsJsonAsync(url, factura);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<RespuestaModel>();

                        if (result != null && result.Indicador)
                        {
                            return Json(new
                            {
                                success = true,
                                message = "El caso ha sido finalizado correctamente",
                                redirectUrl = Url.Action("ListaCasosPendientes", "CasoAtendido")
                            });
                        }
                        return Json(new
                        {
                            success = false,
                            message = result?.Mensaje ?? "No se pudo completar la operación"
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            message = $"Error en el servidor: {errorContent}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }


    }
}
