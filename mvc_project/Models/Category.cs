using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Product>? Products { get; set; }
        public string? ImageUrl { get; set; } 
    }
}
