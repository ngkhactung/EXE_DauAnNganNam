using Microsoft.AspNetCore.Mvc;

namespace DauAnNganNam.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
