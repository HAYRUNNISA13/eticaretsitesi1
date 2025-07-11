using System;
using System.ComponentModel.DataAnnotations;

namespace mvc_project.Models
{
    public class ReturnRequest
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        public string Reason { get; set; } = null!;

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;
        public DateTime? ApprovalDate { get; set; }
    }
}
