using DauAnNganNam.Models;
using System.Collections.Generic;

namespace DauAnNganNam.ViewModel
{
    /// <summary>
    /// ViewModel cho trang danh sách Blog, chứa dữ liệu bài viết và thông tin phân trang.
    /// </summary>
    public class BlogIndexViewModel
    {
        // Danh sách các bài viết sẽ được hiển thị trên trang hiện tại.
        public IEnumerable<News> Blogs { get; set; }

        // Chỉ số của trang hiện tại.
        public int PageIndex { get; set; }

        // Tổng số trang.
        public int TotalPages { get; set; }

        // Thuộc tính kiểm tra xem có trang trước đó không.
        public bool HasPreviousPage => PageIndex > 1;

        // Thuộc tính kiểm tra xem có trang kế tiếp không.
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
