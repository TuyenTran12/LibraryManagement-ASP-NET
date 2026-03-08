using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWeb.Models
{
    public class ChapterImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? ImageUrl { get; set; } 

        [Required]
        public int PageNumber { get; set; } 

        [Required]
        public int ChapterId { get; set; }

        [ForeignKey("ChapterId")]
        public Chapter? Chapter { get; set; }
    }
}