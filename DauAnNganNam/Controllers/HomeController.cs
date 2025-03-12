using Microsoft.AspNetCore.Mvc;

namespace DauAnNganNam.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
