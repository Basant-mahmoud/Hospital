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
        [Required]
        public int DoctorId { get; set; }

        [Required, StringLength(20)]
        public string DayOfWeek { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }

}
