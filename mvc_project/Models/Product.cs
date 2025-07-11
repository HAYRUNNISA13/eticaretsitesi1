using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? CampaignId { get; set; }
        public Campaign? Campaign { get; set; }
        [Required(ErrorMessage = "Tedarikçi seçilmelidir.")]
        public string SupplierId { get; set; }

        public virtual AppUser? Supplier { get; set; }
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    }
}
