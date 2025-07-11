using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


namespace mvc_project.Models
{
    public enum OrderStatus
{
    Pending,    
    Preparing,  
    Shipped,    
    Delivered,  
    Cancelled ,
    ReturnRequested,
     Returned,
     Confirmed
}
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
         public AppUser User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string? Address { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? ShippedDate { get; set; }

    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
