using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.SupportTicket
{
    public class UpdateSupportTicketDto
    {
        public int TicketId { get; set; }
        public string? Response { get; set; }
        public TicketStatus? Status { get; set; }
    }
}
