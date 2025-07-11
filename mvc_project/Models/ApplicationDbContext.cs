using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc_project.Models;

namespace mvc_project.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<IncomingStock> IncomingStocks { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<ReturnRequest> ReturnRequests { get; set; }
        public DbSet<SupplierOrder> SupplierOrders {get; set;}
        public DbSet<SupplierOrderItem> SupplierOrderItems {get; set;}
        public DbSet<AppUser> AppUsers {get; set;}


        
    }
}
