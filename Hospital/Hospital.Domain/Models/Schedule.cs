using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class Schedule
    {

        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        [Required, StringLength(20)]
        public string DayOfWeek { get; set; } = null!;  // Example: "Monday"

        [Required]
        public AppointmentShift Shift { get; set; }     // Morning / Afternoon / Evening

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}