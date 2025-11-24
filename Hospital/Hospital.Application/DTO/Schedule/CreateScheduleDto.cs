using Hospital.Domain.Enum;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Schedule
{
    public class CreateScheduleDto
    {
        public int DoctorId { get; set; }
        public DateOnly ScheduleDate { get; set; }
        public AppointmentShift AppointmentShift { get; set; }

    }

}
