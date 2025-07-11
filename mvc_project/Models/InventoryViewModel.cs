using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using mvc_project.Models;
namespace mvc_project.Models
{
    public class InventoryViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<IncomingStock> IncomingStocks { get; set; } = new();
    }
}
