using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{

public class AdminDashboardViewModel
{
    public List<ProductStatsViewModel> ProductStats { get; set; }
    
    public List<CategoryOrderStatViewModel> CategoryOrderStats { get; set; }
    public List<ReturnReasonStatViewModel> ReturnReasons { get; set; } 
    public List<ProductReturnStatViewModel> ProductReturns { get; set; } 
}}