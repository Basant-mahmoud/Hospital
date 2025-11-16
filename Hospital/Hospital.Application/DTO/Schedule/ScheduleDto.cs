using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Schedule
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; } = null!; public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}