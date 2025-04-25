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
    public class FacturaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FacturaController> _logger;
        private readonly string _connectionString;

        public FacturaController(IConfiguration configuration, ILogger<FacturaController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentNullException("ConnectionStrings:DefaultConnection", "La cadena de conexión no está configurada");
        }

        // POST: api/Factura/Generar
        [HttpPost("Generar")]
        public async Task<IActionResult> GenerarFactura(FacturaModel factura)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_AgregarFactura", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ID_CasoAtendido", factura.ID_CasoAtendido);
                        command.Parameters.AddWithValue("@Comentario", factura.Comentario ?? (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (reader["ID_Factura"].ToString() == "-1")
                                {
                                    return BadRequest(new
                                    {
                                        Error = reader["Mensaje"].ToString()
                                    });
                                }

                                return Ok(new
                                {
                                    Id = Convert.ToInt64(reader["ID_Factura"]),
                                    Mensaje = reader["Mensaje"].ToString(),
                                    Fecha = DateTime.Now
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de SQL al generar factura");
                return StatusCode(500, new
                {
                    Error = "Error de base de datos",
                    Detalles = sqlEx.Message,
                    NumeroError = sqlEx.Number
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al generar factura");
                return StatusCode(500, new
                {
                    Error = "Error interno del servidor",
                    Detalles = ex.Message
                });
            }

            return BadRequest(new { Error = "No se pudo generar la factura" });
        }

        // GET: api/Factura/ObtenerPorId/5
        [HttpGet("ObtenerPorId/{id}")]
        public async Task<IActionResult> ObtenerFacturaPorId(long id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ObtenerFacturaPorId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID_Factura", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Si es un mensaje de error
                                if (reader.FieldCount == 1 && reader.GetName(0) == "Mensaje")
                                {
                                    return NotFound(new { Mensaje = reader["Mensaje"].ToString() });
                                }

                                return Ok(new
                                {
                                    Id = Convert.ToInt64(reader["ID_Factura"]),
                                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                                    Estado = Convert.ToBoolean(reader["Estado"]),
                                    Comentario = reader["Comentario"]?.ToString(),
                                    CasoAtendido = new
                                    {
                                        Id = Convert.ToInt64(reader["ID_CasoAtendido"]),
                                        FechaAtencion = Convert.ToDateTime(reader["Fecha_Atencion"]),
                                        FechaFinalizado = reader["Fecha_Finalizado"] != DBNull.Value ?
                                            Convert.ToDateTime(reader["Fecha_Finalizado"]) : (DateTime?)null,
                                        DiasAtencion = Convert.ToInt32(reader["DiasAtencion"])
                                    },
                                    Caso = new
                                    {
                                        Id = Convert.ToInt64(reader["ID_Caso"]),
                                        Titulo = reader["TituloCaso"].ToString(),
                                        Descripcion = reader["DescripcionCaso"].ToString()
                                    },
                                    Cliente = new
                                    {
                                        NombreCompleto = reader["Cliente"].ToString(),
                                        Email = reader["EmailCliente"].ToString()
                                    }
                                });
                            }
                            return NotFound(new { Error = "Factura no encontrada" });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de SQL al obtener factura");
                return StatusCode(500, new
                {
                    Error = "Error de base de datos",
                    Detalles = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener factura");
                return StatusCode(500, new
                {
                    Error = "Error interno del servidor",
                    Detalles = ex.Message
                });
            }
        }

        // GET: api/Factura/ObtenerPorCasoAtendido/5
        [HttpGet("ObtenerPorCasoAtendido/{idCasoAtendido}")]
        public async Task<IActionResult> ObtenerFacturaPorCasoAtendido(long idCasoAtendido)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ObtenerFacturaPorCasoAtendido", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID_CasoAtendido", idCasoAtendido);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Si es un mensaje de error
                                if (reader.FieldCount == 1 && reader.GetName(0) == "Mensaje")
                                {
                                    return NotFound(new { Mensaje = reader["Mensaje"].ToString() });
                                }

                                return Ok(new
                                {
                                    Id = Convert.ToInt64(reader["ID_Factura"]),
                                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                                    Estado = reader["EstadoDescripcion"].ToString(),
                                    Comentario = reader["Comentario"]?.ToString(),
                                    CasoAtendido = new
                                    {
                                        Id = Convert.ToInt64(reader["ID_CasoAtendido"]),
                                        FechaAtencion = Convert.ToDateTime(reader["Fecha_Atencion"]),
                                        FechaFinalizado = reader["Fecha_Finalizado"] != DBNull.Value ?
                                            Convert.ToDateTime(reader["Fecha_Finalizado"]) : (DateTime?)null,
                                        DiasAtencion = Convert.ToInt32(reader["DiasAtencion"])
                                    },
                                    Caso = new
                                    {
                                        Id = Convert.ToInt64(reader["ID_Caso"]),
                                        Titulo = reader["TituloCaso"].ToString(),
                                        Descripcion = reader["DescripcionCaso"].ToString()
                                    },
                                    Cliente = new
                                    {
                                        NombreCompleto = reader["Cliente"].ToString(),
                                        Email = reader["EmailCliente"].ToString()
                                    },
                                    TotalArticulosUtilizados = reader["TotalArticulosUtilizados"] != DBNull.Value ?
                                        Convert.ToInt32(reader["TotalArticulosUtilizados"]) : 0
                                });
                            }
                            return NotFound(new { Error = "Factura no encontrada para este caso" });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de SQL al obtener factura por caso atendido");
                return StatusCode(500, new
                {
                    Error = "Error de base de datos",
                    Detalles = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener factura por caso atendido");
                return StatusCode(500, new
                {
                    Error = "Error interno del servidor",
                    Detalles = ex.Message
                });
            }
        }
    }
}
