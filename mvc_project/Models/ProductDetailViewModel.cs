using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using mvc_project.Models;
namespace mvc_project.Models
{
public class ProductDetailViewModel
{
    public Product Product { get; set; }
    public List<ProductReview> Reviews { get; set; }
    public bool HasPurchased { get; set; }
}}
