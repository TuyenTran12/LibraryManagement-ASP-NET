using System.ComponentModel.DataAnnotations;

namespace MyWeb.Models
{
    public class UserBookLibrary
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
    }
}
