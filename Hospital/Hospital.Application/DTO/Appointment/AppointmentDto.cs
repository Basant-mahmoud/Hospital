using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Appointment
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }

        public string Status { get; set; }

        public string? Notes { get; set; }
        public BranchShortDto Branch { get; set; }

    }
    public class BranchShortDto
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
    }
}
