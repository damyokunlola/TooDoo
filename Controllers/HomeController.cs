using Microsoft.AspNetCore.Mvc;

namespace TooDooList.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
