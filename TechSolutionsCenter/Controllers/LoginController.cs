using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechSolutionsCenter.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public IActionResult Login()
        {
            return View();
        }

        // GET: LoginController/Details/5
        public IActionResult Register(int id)
        {
            return View();
        }

        
    }
}
