using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; 
using System.Threading.Tasks;
using System;
using System.Linq;
using mvc_project.Models;


namespace mvc_project.Models
{
   
    public class SupplierOrder
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        public DateTime OrderDate { get; set; }
           public AppUser? User { get; set; }
        public ICollection<SupplierOrderItem> Items { get; set; } = new List<SupplierOrderItem>();
        public string? Address { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
       
        

    }

    public class SupplierOrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public SupplierOrder Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
