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
    public class PerfilController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PerfilController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        [Route("ObtenerPerfil/{idUsuario}")]
        public IActionResult ObtenerPerfil(long idUsuario)
        {
            // Validación: El ID debe ser mayor a 0
            if (idUsuario <= 0)
            {
                return BadRequest(new { mensaje = "El ID del usuario es requerido y debe ser mayor a 0." });
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                try
                {
                    var usuario = context.QueryFirstOrDefault<UsuarioModel>(
                        "ObtenerPerfil",
                        new { ID_Usuario = idUsuario },
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    if (usuario == null)
                    {
                        return NotFound(new { mensaje = "Usuario no encontrado." });
                    }

                    return Ok(usuario);
                }
                catch (SqlException ex)
                {
                    return StatusCode(500, new { mensaje = $"Error de base de datos: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { mensaje = $"Error inesperado: {ex.Message}" });
                }
            }
        }


        [HttpPut]
        [Route("ActualizarPerfil")]
        public IActionResult ActualizarPerfil([FromBody] UsuarioModel model)
        {
            var respuesta = new RespuestaModel();

            if (model == null || model.IdUsuario <= 0)
            {
                return BadRequest(new { mensaje = "El ID del usuario es requerido y debe ser mayor a 0." });
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                try
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@ID_Usuario", model.IdUsuario, DbType.Int64);
                    parametros.Add("@Nombre_Usuario", model.Nombre_Usuario, DbType.String);
                    parametros.Add("@Apellidos", model.Apellidos, DbType.String);
                    parametros.Add("@Telefono", model.Telefono, DbType.String);
                    parametros.Add("@Direccion", model.Direccion, DbType.String);
                    parametros.Add("@Email", model.Email, DbType.String);
                    parametros.Add("@Contrasenna", model.Contrasenna, DbType.String);
                    parametros.Add("@ID_Genero", model.ID_Genero, DbType.Int64);
                    parametros.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    context.Execute("ActualizarPerfil", parametros, commandType: CommandType.StoredProcedure);

                    int resultado = parametros.Get<int>("@Resultado");

                    switch (resultado)
                    {
                        case 1:
                            return Ok(new { mensaje = "Perfil actualizado correctamente." });

                        case -1:
                            return BadRequest(new { mensaje = "El usuario no existe." });

                        case -2:
                            return BadRequest(new { mensaje = "El correo electrónico ya está registrado por otro usuario." });

                        case -3:
                            return BadRequest(new { mensaje = "No se realizaron cambios en la actualización." });

                        default:
                            return StatusCode(500, new { mensaje = "Error desconocido en la actualización." });
                    }
                }
                catch (SqlException ex)
                {
                    return StatusCode(500, new { mensaje = $"Error de base de datos: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { mensaje = $"Error inesperado: {ex.Message}" });
                }
            }
        }




        [HttpDelete]
        [Route("EliminarPerfil/{IdUsuario}")]
        public IActionResult EliminarPerfil(long IdUsuario)
        {
            var respuesta = new RespuestaModel();

            // Validación: Verificar si el ID_Usuario es válido
            if (IdUsuario <= 0)
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = "El ID del usuario es requerido y debe ser mayor a 0.";
                return BadRequest(respuesta);
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                try
                {
                    var parametros = new { ID_Usuario = IdUsuario };
                    var result = context.Execute("EliminarPerfil", parametros, commandType: System.Data.CommandType.StoredProcedure);

                    respuesta.Indicador = result > 0;
                    respuesta.Mensaje = result > 0 ? "Perfil eliminado correctamente." : "El usuario no existe o no se pudo eliminar.";
                }
                catch (SqlException ex)
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = $"Error de base de datos: {ex.Message}";
                }
                catch (Exception ex)
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = $"Error inesperado: {ex.Message}";
                }

                return Ok(respuesta);
            }
        }

    }
}