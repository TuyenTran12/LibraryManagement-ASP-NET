using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWeb.Models
{
    public class Book
    {
        [Required]public int Id { get; set; }
        [Required] public string? Title { get; set; }
        [Required] public string? Author { get; set; }
        [Required] public int YearPublished { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
