using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.SupportTicket
{
    public class SupportTicketDto
    {
        public int TicketId { get; set; }
        public string PatientId { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string PatientEmail { get; set; } = null!;  
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Response { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
