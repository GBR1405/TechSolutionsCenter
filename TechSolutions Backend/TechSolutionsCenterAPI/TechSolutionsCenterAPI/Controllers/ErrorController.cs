using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenterAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("CapturarError")]
        public IActionResult CapturarError()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var respuesta = new RespuestaModel();

            respuesta.Indicador = false;
            respuesta.Mensaje = "Se presentó un problema en el sistema, intenta más tarde.";

            return Ok(respuesta);
        }
    }

}
