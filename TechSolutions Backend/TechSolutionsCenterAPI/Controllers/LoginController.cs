using Dapper;
using TechSolutionsCenterAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        //Registrar la cuenta

        [HttpPost]
        [Route("RegistrarCuenta")]
        public IActionResult RegistrarCuenta(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Execute("RegistrarCuenta",
                            new
                            {
                                Nombre_Usuario = model.NombreUsuario,  
                                model.Apellidos,
                                model.Telefono,
                                model.Direccion,
                                model.Email,
                                ID_Genero = model.IdGenero,
                                ID_Rol = model.IdRol,
                                model.Contrasenna
                            });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha registrado correctamente, intente más tarde";
                }

                return Ok(respuesta);
            }
        }

        //Iniciar Sesión

        [HttpPost]
        [Route("IniciarSesion")]
        public IActionResult IniciarSesion(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.QueryFirstOrDefault<UsuarioModel>("IniciarSesion",
                    new { model.Email, model.Contrasenna });

                var respuesta = new RespuestaModel();

                if (result != null)
                {
                    result.Token = GenerarToken(result.IdUsuario, result.IdRol);

                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha validado correctamente, intente más tarde";
                }

                return Ok(respuesta);
            }
        }

        //Recuperar la contraseña

        private string GenerarToken(long IdUsuario, long IdRol)
        {
            string SecretKey = _configuration.GetSection("Variables:llaveToken").Value!;

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("IdUsuario", IdUsuario.ToString()));
            claims.Add(new Claim("IdRol", IdRol.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
