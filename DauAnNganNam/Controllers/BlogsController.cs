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
            int pageSize = 6;

            var totalBlogs = await _context.News.CountAsync();

            var blogs = await _context.News
                .OrderByDescending(n => n.CreateDate) // <-- Sử dụng CreateDate
                .Include(n => n.Images) // <-- Include cả danh sách Images để không bị lỗi khi truy cập
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // *** Dòng quan trọng: Tạo một viewModel mới ***
            var viewModel = new BlogIndexViewModel
            {
                Blogs = blogs, // <-- Gán danh sách blogs đã lấy được
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling(totalBlogs / (double)pageSize)
            };

            // *** Dòng quan trọng: Trả về viewModel, KHÔNG phải danh sách blogs ***
            return View(viewModel);
        }
        // =================================================================

        public async Task<IActionResult> NewsDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include Images khi lấy chi tiết blog
            var blog = await _context.News
                .Include(n => n.Images)
                .FirstOrDefaultAsync(m => m.NewsId == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog); // Giả định NewsDetails.cshtml sử dụng model News
        }
    }
}
