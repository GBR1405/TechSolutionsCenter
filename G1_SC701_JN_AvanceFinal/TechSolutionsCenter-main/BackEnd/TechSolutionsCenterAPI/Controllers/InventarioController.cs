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
    public class InventarioController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<InventarioController> _logger;

        public InventarioController(IConfiguration configuration, ILogger<InventarioController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        #region Inventario

        [HttpPost]
        [Route("AgregarInventario")]
        public IActionResult AgregarInventario([FromBody] InventarioModel model)
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
                        model.N_Lote,
                        model.Cantidad,
                        model.ID_Articulo
                    };

                    var resultado = connection.QueryFirstOrDefault<dynamic>(
                        "sp_AgregarInventario",
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    if (resultado.ID_Inventario == -1)
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
                        Datos = new { ID_Inventario = resultado.ID_Inventario }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar inventario");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al agregar el inventario: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerInventarioPorId/{id}")]
        public IActionResult ObtenerInventarioPorId(long id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultado = connection.QueryFirstOrDefault<dynamic>(
                        "sp_ObtenerInventarioPorId",
                        new { ID_Inventario = id },
                        commandType: CommandType.StoredProcedure);

                    if (resultado == null)
                    {
                        return NotFound(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = "No se encontró el registro de inventario"
                        });
                    }

                    if (resultado.Mensaje != null)
                    {
                        return NotFound(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultado.Mensaje
                        });
                    }

                    var inventario = new
                    {
                        Id = resultado.ID_Inventario,
                        Lote = resultado.N_Lote,
                        Cantidad = resultado.Cantidad,
                        Articulo = new
                        {
                            Id = resultado.ID_Articulo,
                            Nombre = resultado.NombreArticulo,
                            Precio = resultado.Precio,
                            Marca = resultado.Marca,
                            Tipo = resultado.Tipo
                        }
                    };

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Datos = inventario
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener inventario con ID {id}");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener el inventario: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerInventarioDisponible")]
        public IActionResult ObtenerInventarioDisponible()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var inventario = connection.Query<dynamic>(
                        "sp_ObtenerInventarioDisponible",
                        commandType: CommandType.StoredProcedure);

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Datos = inventario
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inventario disponible");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener el inventario disponible: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerTodoElInventario")]
        public IActionResult ObtenerTodoElInventario()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var inventario = connection.Query<dynamic>(
                        "sp_ObtenerTodoElInventario",
                        commandType: CommandType.StoredProcedure);

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Datos = inventario
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todo el inventario");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener todo el inventario: {ex.Message}"
                });
            }
        }

        [HttpPut]
        [Route("ActualizarCantidadInventario/{id}")]
        public IActionResult ActualizarCantidadInventario(long id, [FromBody] int cantidad)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultado = connection.QueryFirstOrDefault<dynamic>(
                        "sp_ActualizarCantidadInventario",
                        new
                        {
                            ID_Inventario = id,
                            Cantidad = cantidad
                        },
                        commandType: CommandType.StoredProcedure);

                    if (resultado.Resultado == -1 || resultado.Resultado == 0)
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
                        Mensaje = resultado.Mensaje
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar cantidad de inventario con ID {id}");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al actualizar la cantidad de inventario: {ex.Message}"
                });
            }
        }



        #endregion

        #region Inventario Utilizado

        [HttpGet]
        [Route("Utilizado/ObtenerPorCaso/{idCasoAtendido}")]
        public IActionResult ObtenerInventarioUtilizadoPorCaso(long idCasoAtendido)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultados = connection.Query<dynamic>(
                        "sp_ObtenerInventarioUtilizado",
                        new { ID_CasoAtendido = idCasoAtendido },
                        commandType: CommandType.StoredProcedure).ToList();

                    if (resultados.Count == 0)
                    {
                        return NotFound(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = "No se encontraron registros para este caso"
                        });
                    }

                    if (resultados[0].Mensaje != null)
                    {
                        return NotFound(new RespuestaModel
                        {
                            Indicador = false,
                            Mensaje = resultados[0].Mensaje
                        });
                    }

                    var lista = resultados.Select(r => new
                    {
                        Id = r.ID_InventarioUtilizado,
                        Fecha = r.Fecha,
                        Cantidad = r.Cantidad,
                        Inventario = new
                        {
                            Id = r.ID_Inventario,
                            Lote = r.N_Lote
                        },
                        Articulo = new
                        {
                            Nombre = r.NombreArticulo,
                            Precio = r.Precio
                        },
                        Caso = new
                        {
                            Titulo = r.TituloCaso,
                            FechaAtencion = r.Fecha_Atencion
                        }
                    }).ToList();

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Datos = lista
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener inventario utilizado para caso {idCasoAtendido}");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener el inventario utilizado: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Route("GestionarInventarioUtilizado")]
        public IActionResult GestionarInventarioUtilizado([FromBody] InventarioUtilizadoModel model)
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
                    var resultado = connection.QueryFirstOrDefault<dynamic>(
                        "sp_GestionarInventarioUtilizado",
                        new
                        {
                            ID_Inventario = model.ID_Inventario,
                            ID_CasoAtendido = model.ID_CasoAtendido
                        },
                        commandType: CommandType.StoredProcedure);

                    if (resultado.Resultado == -1)
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
                        Datos = new
                        {
                            ID_InventarioUtilizado = resultado.Resultado,
                            Accion = resultado.Resultado == 1 ? "Actualizado" : "Creado"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al gestionar inventario utilizado");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al gestionar el inventario utilizado: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Route("RestarInventarioUtilizado")]
        public IActionResult RestarInventarioUtilizado([FromBody] InventarioUtilizadoModel model)
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
                    var resultado = connection.QueryFirstOrDefault<dynamic>(
                        "sp_RestarInventarioUtilizado",
                        new
                        {
                            ID_CasoAtendido = model.ID_CasoAtendido,
                            ID_Inventario = model.ID_Inventario
                        },
                        commandType: CommandType.StoredProcedure);

                    if (resultado.Resultado == -1)
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
                        Datos = new
                        {
                            NuevaCantidad = resultado.Resultado,
                            Accion = resultado.Resultado == 0 ? "Eliminado" : "Actualizado"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al restar inventario utilizado para caso {model.ID_CasoAtendido} e inventario {model.ID_Inventario}");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al restar el inventario utilizado: {ex.Message}"
                });
            }
        }

        [HttpDelete]
        [Route("Utilizado/EliminarInventarioUtilizado/{id}")]
        public IActionResult EliminarInventarioUtilizado(long id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var resultado = connection.QueryFirstOrDefault<dynamic>(
                        "sp_EliminarInventarioUtilizado",
                        new { ID_InventarioUtilizado = id },
                        commandType: CommandType.StoredProcedure);

                    if (resultado.Resultado == -1 || resultado.Resultado == 0)
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
                        Datos = new { FilasAfectadas = resultado.Resultado }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar inventario utilizado con ID {id}");
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al eliminar el inventario utilizado: {ex.Message}"
                });
            }
        }

        #endregion
    }
}