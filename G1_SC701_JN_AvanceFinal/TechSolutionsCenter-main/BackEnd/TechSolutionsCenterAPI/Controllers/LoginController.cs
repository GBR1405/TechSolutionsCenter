using Dapper;
using TechSolutionsCenterAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Data;

namespace TechSolutionsCenterAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Registrar la cuenta
        [HttpPost]
        [Route("RegistrarCuenta")]
        public IActionResult RegistrarCuenta(UsuarioModel model)
        {
            var respuesta = new RespuestaModel();

            try
            {
                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    // Usar QueryFirstOrDefault para capturar posibles mensajes de error del SP
                    var result = context.QueryFirstOrDefault<dynamic>("RegistrarCuenta",
                        new
                        {
                            Nombre_Usuario = model.Nombre_Usuario,
                            model.Apellidos,
                            model.Telefono,
                            model.Direccion,
                            model.Email,
                            ID_Genero = model.ID_Genero,
                            model.Contrasenna,
                            ID_Rol = 2,
                        },
                        commandType: CommandType.StoredProcedure);

                    // Asumimos que el SP devuelve un campo Indicador y Mensaje
                    if (result != null && result.Indicador == true)
                    {
                        respuesta.Indicador = true;
                        respuesta.Mensaje = "Registro exitoso";
                    }
                    else
                    {
                        respuesta.Indicador = false;
                        respuesta.Mensaje = result?.Mensaje ?? "Su información no se ha registrado correctamente, intente más tarde";
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo específico de errores SQL
                respuesta.Indicador = false;
                respuesta.Mensaje = $"Error de base de datos: {sqlEx.Message}";

            }
            catch (Exception ex)
            {
                // Manejo de otros errores
                respuesta.Indicador = false;
                respuesta.Mensaje = $"Error inesperado: {ex.Message}";
            }

            return Ok(respuesta);
        }

        // Iniciar Sesión
        [HttpPost]
        [Route("IniciarSesion")]
        public IActionResult IniciarSesion(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var respuesta = new RespuestaModel();
                try
                {
                    var result = context.QueryFirstOrDefault("IniciarSesion",
                         new { model.Email, model.Contrasenna },
                         commandType: System.Data.CommandType.StoredProcedure);

                    if (result != null && Convert.ToInt32(result.Indicador) == 1)
                    {
                        // Convertir los IDs a long
                        long idUsuario = Convert.ToInt64(result.ID_Usuario);
                        long idRol = Convert.ToInt64(result.ID_Rol);
                        string correo = (result.Email);

                        // Generar el token
                        result.Token = GenerarToken(idUsuario, idRol, correo);
                        respuesta.Indicador = true;
                        respuesta.Datos = result;
                    }
                    else
                    {
                        respuesta.Indicador = false;
                        respuesta.Mensaje = result?.Mensaje ?? "Error desconocido";
                    }
                }
                catch (SqlException ex)
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = ex.Message;
                }
                catch (Exception ex)
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = $"Error inesperado: {ex.Message}";
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("RecuperarContrasenna")]
        public IActionResult RecuperarContrasenna(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.QueryFirstOrDefault<UsuarioModel>("ValidarUsuarioCorreo",
                    new { model.Email });

                var respuesta = new RespuestaModel();

                if (result != null)
                {
                    var Codigo = GenerarCodigo();
                    var Contrasenna = Encrypt(Codigo);
                    var ContrasennaAnterior = string.Empty;

                    //Actualizar contraseña
                    context.Execute("ActualizarContrasenna",
                        new { result.Email, Contrasenna});

                    //Notificar al usuario
                    string Contenido = "Hola " + result.Nombre_Usuario + " " + result.Apellidos + ". Se ha generado el siguiente código de seguridad: " + Codigo;
                    EnviarCorreo(result.Email!, "Actualización de Acceso", Contenido);

                    respuesta.Indicador = true;
                    respuesta.Datos = result; 
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha validado correctamente";
                }

                return Ok(respuesta);
            }
        }



        // Recuperar la contraseña
        private string GenerarToken(long IdUsuario, long IdRol, string email)
        {
            string SecretKey = _configuration.GetSection("Variables:llaveToken").Value!;

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("IdUsuario", IdUsuario.ToString()));
            claims.Add(new Claim("IdRol", IdRol.ToString()));
            claims.Add(new Claim("Correo", email));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerarCodigo()
        {
            int length = 8;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_configuration.GetSection("Variables:llaveCifrado").Value!);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private void EnviarCorreo(string destino, string asunto, string contenido)
        {
            string cuenta = _configuration.GetSection("Variables:CorreoEmail").Value!;
            string contrasenna = _configuration.GetSection("Variables:ClaveEmail").Value!;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(cuenta);
            message.To.Add(new MailAddress(destino));
            message.Subject = asunto;
            message.Body = contenido;
            message.Priority = MailPriority.Normal;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.Credentials = new System.Net.NetworkCredential(cuenta, contrasenna);
            client.EnableSsl = true;

            //Esto es para que no se intente enviar el correo si no hay una contraseña
            if (!string.IsNullOrEmpty(contrasenna))
            {
                client.Send(message);
            }
        }
    }
}
