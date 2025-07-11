using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public decimal DiscountRate { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
