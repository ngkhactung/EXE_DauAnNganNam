using DauAnNganNam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DauAnNganNam.Controllers
{
    public class HomeController : Controller
    {
        public readonly ExeDBContext _context;
        public HomeController(ExeDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(x => x.Images);
            ViewBag.Products = products;
            return View();
        }
    }
}
