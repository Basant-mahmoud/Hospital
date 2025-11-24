using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Schedule
{
    public class UpdateScheduleDto
    {
        public int ScheduleId { get; set; }
        public string? DayOfWeek { get; set; }
        public AppointmentShift? AppointmentShift { get; set; }
    }
}