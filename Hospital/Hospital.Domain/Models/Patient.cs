using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class Patient
    {
        [Key] public int PatientId { get; set; }
        [Required] public string UserId { get; set; }
        public User User { get; set; } = null!;
        [StringLength(5)] public string? BloodType { get; set; }
        [StringLength(150)] public string? EmergencyContact { get; set; }
        [StringLength(250)] public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
