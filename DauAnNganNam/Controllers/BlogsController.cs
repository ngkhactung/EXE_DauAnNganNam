using DauAnNganNam.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace DauAnNganNam.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ExeDBContext _context;

        public BlogsController(ExeDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult LoginToCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginToCreate(Model model)
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateBlogs()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateBlogs(News blog, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64Image = Convert.ToBase64String(imageBytes);
                    blog.Content += $"<p><img src='data:image/jpeg;base64,{base64Image}'/></p>";
                }
            }

            // Lưu vào cơ sở dữ liệu
            _context.News.Add(blog);
            _context.SaveChanges();

            return RedirectToAction(nameof(CreateBlogs));
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                try
                {
                    // Kiểm tra kiểu file (chỉ cho phép ảnh)
                    if (upload.ContentType.StartsWith("image/"))
                    {
                        // Lấy tên file ảnh
                        var fileName = Path.GetFileName(upload.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        // Kiểm tra nếu ảnh đã tồn tại
                        if (System.IO.File.Exists(filePath))
                        {
                            fileName = Path.GetFileNameWithoutExtension(upload.FileName) + Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                        }

                        // Lưu ảnh vào thư mục wwwroot/images
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            upload.CopyTo(fileStream);
                        }

                        // Trả về URL ảnh sau khi upload thành công
                        var url = Url.Content($"~/images/{fileName}");
                        return Json(new { uploaded = true, url = url });
                    }
                    else
                    {
                        return Json(new { uploaded = false, error = "Only image files are allowed." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { uploaded = false, error = ex.Message });
                }
            }
            else
            {
                return Json(new { uploaded = false, error = "No file uploaded." });
            }
        }

        public IActionResult Index()
        {
            var blogs = _context.News.ToList();
            return View(blogs);
        }
    }
}
