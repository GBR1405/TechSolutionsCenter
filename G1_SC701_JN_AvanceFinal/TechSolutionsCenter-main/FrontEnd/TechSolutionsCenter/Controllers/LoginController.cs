using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using TechSolutionsCenter.Models;
using JN_ProyectoWeb.Models;

namespace TechSolutionsCenter.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        public LoginController(IHttpClientFactory httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        #region RegistrarCuenta

        [HttpGet]
        public IActionResult RegistrarCuenta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarCuenta(UsuarioModel model)
        {
            model.Contrasenna = Encrypt(model.Contrasenna!);

            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Login/RegistrarCuenta";
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                     return RedirectToAction("IniciarSesion", "Login");
            }

            return View();
        }

        #endregion

        #region IniciarSesion

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(UsuarioModel model)
        {
            model.Contrasenna = Encrypt(model.Contrasenna!);

            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Login/IniciarSesion";
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador && result.Datos != null)
                    {
                        var datosResult = JsonSerializer.Deserialize<UsuarioModel>((JsonElement)result.Datos!);

                        if (datosResult != null)
                        {
                            // Guardar solo si los valores no son nulos
                            HttpContext.Session.SetString("Token", datosResult.Token ?? "");
                            HttpContext.Session.SetString("IdUsuario", datosResult.ID_Usuario.ToString() ?? "");
                            string nombreCompleto = $"{datosResult.Nombre_Usuario} {datosResult.Apellidos}";
                            HttpContext.Session.SetString("NombreUsuario", nombreCompleto);
                            HttpContext.Session.SetString("Email", datosResult.Email ?? "");
                            HttpContext.Session.SetString("IdGenero", datosResult.ID_Genero.ToString() ?? "");
                            HttpContext.Session.SetString("IdRol", datosResult.ID_Rol.ToString() ?? "");
                            HttpContext.Session.SetString("NombreRol", datosResult.NombreRol ?? "");

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }

            return View();
        }


        #endregion

        [FiltroSesion]
        [HttpGet]
        public IActionResult Principal()
        {
            return View();
        }

        #region RecuperarContrasenna

        [HttpGet]
        public IActionResult RecuperarContrasenna()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarContrasenna(UsuarioModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Login/RecuperarContrasenna";
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                        return RedirectToAction("IniciarSesion", "Login");
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    TempData["Mensaje"] = "Error al generar la peticiond de cambio de contraseña, verifique su correo";
            }
            TempData["Mensaje"] = "Error al generar la peticion de cambio, verifique su correo";
            return View();
        }

        #endregion

        #region Filtros

        [FiltroSesion]
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("IniciarSesion", "Login");
        }

        #endregion

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

        private string Decrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(texto);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_configuration.GetSection("Variables:llaveCifrado").Value!);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
