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
   
public class SupplierOrderItemInput
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}}