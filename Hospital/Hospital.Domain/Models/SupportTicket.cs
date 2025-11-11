using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class SupportTicket
    {
        [Key] public int TicketId { get; set; }
        [Required] public string UserId { get; set; }
        public User User { get; set; } = null!;
        [Required, StringLength(200)] public string Subject { get; set; } = null!;
        [Required] public string Message { get; set; } = null!;
        [Required] public TicketStatus Status { get; set; } = TicketStatus.Open;
        public string? Response { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
