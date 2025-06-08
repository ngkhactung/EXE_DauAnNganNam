using System.ComponentModel.DataAnnotations;

namespace DauAnNganNam.Models
{
    public class Blogs
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        // Thêm thuộc tính để lưu URL của hình ảnh
        public string ImageUrl { get; set; }
    }
}
