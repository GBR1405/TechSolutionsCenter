using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenterAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticuloController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ArticuloController> _logger;

        public ArticuloController(IConfiguration configuration, ILogger<ArticuloController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("AgregarArticulo")]
        public async Task<IActionResult> AgregarArticulo([FromBody] ArticuloModel model)
        {
            if (model == null)
            {
                return BadRequest(new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = "El cuerpo de la solicitud no puede estar vacío"
                });
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var parametros = new
                    {
                        model.Nombre,
                        model.Precio,
                        model.ID_Marca,
                        model.ID_Tipo
                    };

                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_AgregarArticulo",
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    // Manejo de errores desde el SP
                    if (resultado != null && resultado.ErrorNumber != null)
                    {
                        _logger.LogError($"Error al agregar artículo: {resultado.ErrorMessage}");
                        return BadRequest(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.ErrorMessage,
                            Datos = new { CodigoError = resultado.ErrorNumber }
                        });
                    }

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Mensaje = "Artículo agregado correctamente",
                        Datos = new { ID_Articulo = resultado.ID_Articulo }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar artículo");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al agregar el artículo: {ex.Message}"
                });
            }
        }

        [HttpPut]
        [Route("ActualizarArticulo/{id}")]
        public async Task<IActionResult> ActualizarArticulo(long id, [FromBody] ArticuloModel model)
        {
            if (model == null)
            {
                return BadRequest(new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = "El cuerpo de la solicitud no puede estar vacío"
                });
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var parametros = new
                    {
                        ID_Articulo = id,
                        model.Nombre,
                        model.Precio,
                        model.ID_Marca,
                        model.ID_Tipo
                    };

                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_ActualizarArticulo",
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    // Manejo de errores desde el SP
                    if (resultado != null && resultado.ErrorNumber != null)
                    {
                        _logger.LogError($"Error al actualizar artículo: {resultado.ErrorMessage}");
                        return BadRequest(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.ErrorMessage,
                            Datos = new { CodigoError = resultado.ErrorNumber }
                        });
                    }

                    if (resultado.Resultado > 0)
                    {
                        return Ok(new RespuestaModel
                        {
                            Indicador = true,
                            Mensaje = "Artículo actualizado correctamente",
                            Datos = new { FilasAfectadas = resultado.Resultado }
                        });
                    }

                    return NotFound(new RespuestaModel
                    {
                        Indicador = false,
                        Mensaje = "No se encontró el artículo para actualizar"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar artículo");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al actualizar el artículo: {ex.Message}"
                });
            }
        }

        [HttpDelete]
        [Route("ObtenerArticulos")]
        public async Task<IActionResult> ObtenerArticulos(long id)
        {
            try
            {
                var respuesta = new RespuestaModel();

                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    var result = context.Query<ArticuloModel>("sp_ObtenerArticulos");

                    if (result.Any())
                    {
                        respuesta.Indicador = true;
                        respuesta.Datos = result;
                    }
                    else
                    {
                        respuesta.Indicador = false;
                        respuesta.Mensaje = "No hay información registrada";
                    }

                    return Ok(respuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar artículo");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al eliminar el artículo: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerArticuloPorId/{id}")]
        public async Task<IActionResult> ObtenerArticuloPorId(long id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_ObtenerArticuloPorId",
                        new { ID_Articulo = id },
                        commandType: CommandType.StoredProcedure);

                    // Manejo de errores desde el SP
                    if (resultado != null && resultado.ErrorNumber != null)
                    {
                        _logger.LogError($"Error al obtener artículo: {resultado.ErrorMessage}");
                        return BadRequest(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.ErrorMessage,
                            Datos = new { CodigoError = resultado.ErrorNumber }
                        });
                    }

                    if (resultado != null)
                    {
                        var articulo = new
                        {
                            Id = resultado.ID_Articulo,
                            Nombre = resultado.Nombre,
                            Precio = resultado.Precio,
                            Marca = new
                            {
                                Id = resultado.ID_Marca,
                                Nombre = resultado.NombreMarca
                            },
                            Tipo = new
                            {
                                Id = resultado.ID_Tipo,
                                Nombre = resultado.NombreTipo
                            }
                        };

                        return Ok(new RespuestaModel
                        {
                            Indicador = true,
                            Datos = articulo
                        });
                    }

                    return NotFound(new RespuestaModel
                    {
                        Indicador = false,
                        Mensaje = "Artículo no encontrado"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículo por ID");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener el artículo: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerArticuloS")]
        public async Task<IActionResult> ObtenerArticulos()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultados = await connection.QueryAsync<dynamic>(
                        "sp_ObtenerArticulos",
                        commandType: CommandType.StoredProcedure);

                    if (resultados == null || !resultados.Any())
                    {
                        return NotFound(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = "No se encontraron artículos"
                        });
                    }

                    var articulos = resultados.Select(r => new InventarioModel
                    {
                        ID_Articulo = r.ID_Articulo,
                        NombreArticulo = r.Nombre,
                        Precio = r.Precio,


                            Marca = r.NombreMarca,
                            Tipo = r.NombreTipo
                    }).ToList();

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Datos = articulos
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener artículos: {ex.Message}"
                });
            }
        }
    }
}