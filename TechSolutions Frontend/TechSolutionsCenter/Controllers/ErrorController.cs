using Microsoft.AspNetCore.Mvc;

namespace TechSolutionsCenter.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult CapturarError()
        {
            return View("Error");
        }
    }
}
