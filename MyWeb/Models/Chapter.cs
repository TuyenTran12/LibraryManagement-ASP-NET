using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWeb.Models
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; } // Ví dụ: "Chapter 1: Sự khởi đầu"

        [Required]
        public double ChapterNumber { get; set; } // Dùng double vì có thể có chapter 1.5

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày đăng chương

        // Khóa ngoại nối về Book (Manga)
        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        // 1 Chương có nhiều Trang ảnh
        public List<ChapterImage>? Images { get; set; }
    }
}