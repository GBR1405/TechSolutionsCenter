using Dapper;
using JN_ProyectoApi.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenterAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CasoAtendidoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CasoAtendidoController> _logger;
        private readonly IGeneral _general;

        public CasoAtendidoController(IConfiguration configuration, ILogger<CasoAtendidoController> logger, IGeneral general)
        {
            _configuration = configuration;
            _logger = logger;
            _general = general;
        }

        [HttpPost]
        [Route("AgregarCasoAtendido")]
        public async Task<IActionResult> AgregarCasoAtendido(CasoAtendidoModel Modelo)
        {
            long ID_Usuario = _general.ObtenerUsuarioFromToken(User.Claims);

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var parametros = new
                    {
                        Modelo.ID_Caso,
                        ID_Usuario
                    };

                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_AgregarCasoAtendido",
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    if (resultado.ID_CasoAtendido == -1)
                    {
                        return BadRequest(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.Mensaje
                        });
                    }

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Mensaje = resultado.Mensaje,
                        Datos = new { ID_CasoAtendido = resultado.ID_CasoAtendido }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar caso atendido");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al registrar el caso atendido: {ex.Message}"
                });
            }
        }

        [HttpPut]
        [Route("ActualizarCasoAtendido/{id}")]
        public async Task<IActionResult> ActualizarCasoAtendido(long id, [FromBody] CasoAtendidoModel model)
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
                        ID_CasoAtendido = id,
                        model.Fecha_Finalizado
                    };

                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_ActualizarCasoAtendido",
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    if (resultado.ID_CasoAtendido == -1)
                    {
                        return BadRequest(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.Mensaje
                        });
                    }

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Mensaje = resultado.Mensaje,
                        Datos = new { ID_CasoAtendido = resultado.ID_CasoAtendido }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar caso atendido con ID {id}");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al actualizar el caso atendido: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerCasoAtendidoPorId")]
        public async Task<IActionResult> ObtenerCasoAtendidoPorId()
        {
            long idUsuario = _general.ObtenerUsuarioFromToken(User.Claims);
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_ObtenerCasoAtendidoPorId",
                        new { ID_Usuario = idUsuario },
                        commandType: CommandType.StoredProcedure);

                    // Manejo de errores desde el SP
                    if (resultado != null && resultado.ErrorNumber != null)
                    {
                        _logger.LogError($"Error al obtener caso atendido: {resultado.ErrorMessage}");
                        return BadRequest(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.ErrorMessage,
                            Datos = new { CodigoError = resultado.ErrorNumber }
                        });
                    }

                    if (resultado != null)
                    {
                        if (resultado.Mensaje != null) // Si hay mensaje es porque no se encontró
                        {
                            return NotFound(new RespuestaModel
                            {
                                Indicador = false,
                                Mensaje = resultado.Mensaje
                            });
                        }

                        return Ok(new RespuestaModel
                        {
                            Indicador = true,
                            Datos = resultado
                        });
                    }

                    return NotFound(new RespuestaModel
                    {
                        Indicador = false,
                        Mensaje = "Caso atendido no encontrado"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener caso atendido con ID");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener el caso atendido: {ex.Message}"
                });
            }
        }


    }
}