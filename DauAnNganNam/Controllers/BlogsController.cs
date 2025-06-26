using DauAnNganNam.Models;
using DauAnNganNam.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DauAnNganNam.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ExeDBContext _context;

        public BlogsController(ExeDBContext context)
        {
            _context = context;
        }

        // =================================================================
        // == ĐÂY LÀ PHẦN QUAN TRỌNG CẦN KIỂM TRA LẠI VÀ SỬA ĐÚNG
        // =================================================================
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            return View();
        }
        // =================================================================

    }
}
