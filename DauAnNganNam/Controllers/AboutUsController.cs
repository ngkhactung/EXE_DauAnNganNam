using Microsoft.AspNetCore.Mvc;

namespace DauAnNganNam.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
