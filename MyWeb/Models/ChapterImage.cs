using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWeb.Models
{
    public class ChapterImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? ImageUrl { get; set; } // Sẽ chứa link ảnh từ Cloudinary

        [Required]
        public int PageNumber { get; set; } // Số thứ tự trang (1, 2, 3...) để sắp xếp ảnh cho đúng

        // Khóa ngoại nối về Chapter
        [Required]
        public int ChapterId { get; set; }

        [ForeignKey("ChapterId")]
        public Chapter? Chapter { get; set; }
    }
}