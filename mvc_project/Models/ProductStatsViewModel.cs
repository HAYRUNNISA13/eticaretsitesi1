using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace mvc_project.Models
{
    public class ProductStatsViewModel
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
        public int FavoriteCount { get; set; }
    }
}
