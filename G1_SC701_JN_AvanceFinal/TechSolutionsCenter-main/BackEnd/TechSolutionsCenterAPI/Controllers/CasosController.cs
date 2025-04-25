using Dapper;
using JN_ProyectoApi.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using TechSolutionsCenter.Models;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenterAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CasosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;

        public CasosController(IConfiguration configuration, IGeneral general)
        {
            _configuration = configuration;
            _general = general;
        }

        [HttpPost]
        [Route("AgregarCaso")]
        public IActionResult AgregarCaso([FromBody] CasosModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "El cuerpo de la solicitud no puede estar vacío" });
            }

            long idUsuario = _general.ObtenerUsuarioFromToken(User.Claims);

            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var parametros = new
                    {
                        model.Titulo,
                        model.Descripcion,
                        Imagen = model.Imagen,
                        ID_Usuario = idUsuario,
                        Estado = model.Estado,
                    };

                    var idCaso = context.ExecuteScalar<long>("sp_AgregarCaso",
                        parametros,
                        commandType: CommandType.StoredProcedure);

                    string correo = _general.ObtenerCorreoFromToken(User.Claims);
                    EnviarCorreo(correo);

                    return Ok(new RespuestaModel
                    {


                        Indicador = true,
                        Mensaje = "Caso registrado exitosamente",
                        Datos = new { ID_Caso = idCaso }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al registrar el caso: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Route("EditarCasoPendiente")]
        public IActionResult EditarCaso([FromBody] EditarCasoRequest request)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var SQLEditar = context.ExecuteScalar<long>("EditarCasoPendiente",
                        new { idCaso = request.idCaso },
                        commandType: CommandType.StoredProcedure);

                    return Ok(new RespuestaModel
                    {
                        Indicador = true,
                        Mensaje = "Caso editado exitosamente",
                        Datos = new { ID_Caso = request.idCaso }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al editar el caso: {ex.Message}"
                });
            }
        }

        // Clase auxiliar para el request
        public class EditarCasoRequest
        {
            public long idCaso { get; set; }
        }

        [HttpGet]
        [Route("ObtenerCasoPorId/{id}")]
        public IActionResult ObtenerCasoPorId(long id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
            {
                var caso = context.QueryFirstOrDefault<CasosModel>(
                    "sp_ObtenerCasoPorId",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure);

                var respuesta = new RespuestaModel();

                if (caso != null)
                {
                    respuesta.Indicador = true;
                    respuesta.Datos = caso;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Caso no encontrado";
                }

                return Ok(respuesta);
            }
        }

        [HttpGet]
        [Route("ObtenerCasosPorUsuario")]
        public IActionResult ObtenerCasosPorUsuario()
        {
            long idUsuario = _general.ObtenerUsuarioFromToken(User.Claims);

            using (var context = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
            {
                var casos = context.Query<CasosModel>(
                    "sp_ObtenerCasosPorUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure);

                var respuesta = new RespuestaModel
                {
                    Indicador = true,
                    Datos = casos
                };

                return Ok(respuesta);
            }
        }

        [HttpGet]
        [Route("ObtenerTodosLosCasos")]
        public IActionResult ObtenerTodosLosCasos()
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var casos = context.Query<dynamic>(
                        "sp_ObtenerTodosLosCasos",
                        commandType: CommandType.StoredProcedure);

                    var respuesta = new RespuestaModel
                    {
                        Indicador = true,
                        Datos = casos
                    };

                    return Ok(respuesta);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener los casos: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("ObtenerTodosLosCasosPendientes")]
        public IActionResult ObtenerCasosEnEstadoPendiente()
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("BDConnection")))
                {
                    var casos = context.Query<dynamic>(
                        "ObtenerCasosEnEstadoEntrantes",
                        commandType: CommandType.StoredProcedure);

                    var respuesta = new RespuestaModel
                    {
                        Indicador = true,
                        Datos = casos
                    };

                    return Ok(respuesta);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaModel
                {
                    Indicador = false,
                    Mensaje = $"Error al obtener los casos: {ex.Message}"
                });
            }
        }

        private void EnviarCorreo(string destino)
        {
            string cuenta = _configuration.GetSection("Variables:CorreoEmail").Value!;
            string contrasenna = _configuration.GetSection("Variables:ClaveEmail").Value!;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(cuenta, "TechSolutionCenter");
            message.To.Add(new MailAddress(destino));
            message.Subject = "Confirmación de registro de caso - TechSolutionCenter";
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;

            string html = @"
    <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
        <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 30px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
            <h2 style='color: #2E86C1; text-align: center;'>¡Tu caso ha sido registrado correctamente!</h2>
            <p style='font-size: 16px; color: #333333;'>Hola,</p>
            <p style='font-size: 16px; color: #333333;'>
                Te informamos que hemos recibido correctamente el registro del caso relacionado con tu equipo.
            </p>
            <p style='font-size: 16px; color: #333333;'>
                <strong>Tienes un plazo de 3 días</strong> a partir de hoy para entregar el equipo en nuestras instalaciones para su revisión técnica.
            </p>
            <p style='font-size: 16px; color: #333333;'>
                Una vez recibido, uno de nuestros técnicos lo evaluará y te mantendremos informado sobre el proceso de reparación.
            </p>
            <div style='margin: 30px 0; text-align: center;'>
                <img src='https://cdn-icons-png.flaticon.com/512/3208/3208716.png' alt='Equipo registrado' width='100' style='margin-bottom: 10px;' />
                <p style='font-size: 14px; color: #777777;'>Gracias por confiar en nosotros.</p>
            </div>
            <p style='font-size: 14px; color: #777777; text-align: center;'>Atentamente,<br><strong>El equipo de TechSolutionCenter</strong></p>
        </div>
    </div>";

            message.Body = html;

            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.Credentials = new System.Net.NetworkCredential(cuenta, contrasenna);
            client.EnableSsl = true;

            if (!string.IsNullOrEmpty(contrasenna))
            {
                client.Send(message);
            }
        }

    }
}
