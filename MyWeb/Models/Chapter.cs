using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWeb.Models
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; } 

        [Required]
        public double ChapterNumber { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now; 

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        public List<ChapterImage>? Images { get; set; }
    }
}