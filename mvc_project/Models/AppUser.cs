using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace mvc_project.Models
{
public class AppUser : IdentityUser
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(150)]
    public string? CompanyName { get; set; }  // Tedarikçi ya da firma kullanıcıları için

    public bool IsSupplier { get; set; }

    // Adres bilgileri (normal kullanıcılar için zorunlu ya da opsiyonel yapabilirsin)
    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    // Telefon numarası zaten IdentityUser'da PhoneNumber olarak var
}}
