using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class MedicalRecord
    {
        [Key] public int RecordId { get; set; }
        [Required] public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;
        [Required] public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        [Required] public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
