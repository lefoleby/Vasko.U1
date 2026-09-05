using Microsoft.AspNetCore.Mvc;

namespace Vasko.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
