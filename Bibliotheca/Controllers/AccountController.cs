using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
