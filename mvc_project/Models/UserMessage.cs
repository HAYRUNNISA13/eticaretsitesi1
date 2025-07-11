using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{
   
public class UserMessage
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string Message { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}}
