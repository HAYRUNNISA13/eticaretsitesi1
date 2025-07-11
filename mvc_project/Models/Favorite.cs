using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
