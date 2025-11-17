using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.SupportTicket
{
    public class CreateSupportTicketDto
    {
        public int PatientId { get; set; }
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
