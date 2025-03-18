using DauAnNganNam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DauAnNganNam.Controllers
{
    public class ProductController : Controller
    {
        public readonly ExeDBContext _context;

        public ProductController(ExeDBContext context)
		{
			_context = context;
		}

		public IActionResult Index()
        {
            var queryProduct = _context.Products.Include(x => x.Images).Include(x => x.ProductCategory).ToList();
            var queryCategory = _context.ProductCategories.ToList();
			ViewBag.Categories = queryCategory;
			ViewBag.Products = queryProduct;
			return View();
        }

        public IActionResult Detail(int id)
        {
            var queryProduct = _context.Products.Include(x => x.Images).Include(x => x.ProductCategory).FirstOrDefault(x => x.ProductId == id);
            ViewBag.Product = queryProduct;
            return View();
        }
    }
}
