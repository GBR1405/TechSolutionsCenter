using Dapper;
using JN_ProyectoApi.Servicios;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using TechSolutionsCenterAPI.Models;

namespace JN_ProyectoApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ErrorController> _logger;
        private readonly string _connectionString;

        public ErrorController(IConfiguration configuration, ILogger<ErrorController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentNullException("ConnectionStrings:DefaultConnection", "La cadena de conexión no está configurada");
        }

        [HttpPost("Registrar")]
        public IActionResult RegistrarError([FromBody] ErrorModel error)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Execute(
                        "dbo.sp_RegistrarError", // Especificar esquema (dbo) explícitamente
                        new
                        {
                            error.Mensaje,
                            error.StackTrace,
                            IdUsuario = error.ID_Usuario, // Asegurar que coincida con el parámetro del SP
                            error.Origen
                        },
                        commandType: CommandType.StoredProcedure);
                }

                return Ok(new { Mensaje = "Error registrado correctamente" });
            }
            catch (SqlException sqlEx) when (sqlEx.Message.Contains("Invalid object name"))
            {
                _logger.LogCritical(sqlEx, "La tabla 'Errores' no existe o no es accesible");
                return StatusCode(500, new
                {
                    Error = "Error de configuración de base de datos",
                    Solucion = "Verificar que la tabla 'dbo.Errores' exista y sea accesible"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el error en la base de datos");
                return StatusCode(500, new
                {
                    Error = "Error interno al registrar el error",
                    Detalles = ex.Message
                });
            }
        }

        [HttpGet("CapturarError")]
        public IActionResult CapturarError()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = new ErrorModel
            {
                Fecha = DateTime.Now,
                Mensaje = ex?.Error.Message ?? "Excepción nula",
                StackTrace = ex?.Error.StackTrace ?? "Stack trace no disponible",
                ID_Usuario = 0, // Valor por defecto o implementa tu lógica de obtención de usuario
                Origen = ex?.Path ?? HttpContext.Request.Path
            };

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Execute(
                        "dbo.sp_RegistrarError",
                        new
                        {
                            error.Mensaje,
                            error.StackTrace,
                            IdUsuario = error.ID_Usuario,
                            error.Origen
                        },
                        commandType: CommandType.StoredProcedure);
                }

                var respuesta = new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = "Se presentó un problema en el sistema."
                };

                return BadRequest(respuesta);
            }
            catch (Exception exDb)
            {
                _logger.LogCritical(exDb, "Fallo al registrar error en la base de datos. Error original: {MensajeError}", error.Mensaje);

                // Respuesta de fallback si no se puede registrar el error
                return StatusCode(500, new
                {
                    Error = error.Mensaje,
                    Detalles = "Adicionalmente, falló el registro del error en el sistema"
                });
            }
        }
    }
}
