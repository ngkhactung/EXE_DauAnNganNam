using Microsoft.AspNetCore.Mvc;

namespace DauAnNganNam.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
