using Hospital.Application.DTO.Branch;
using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Doctor
{
    public class DoctorSchuduleDto
    {
        public int ScheduleId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AppointmentShift Shift { get; set; }
        public BranchDto Branch { get; set; }   
    }
}