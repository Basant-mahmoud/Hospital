using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class Doctor
    {
        [Key] public int DoctorId { get; set; }
        [Required] public string UserId { get; set; }
        public User User { get; set; } = null!;
        [Required] public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; } = null!;
        [Url, StringLength(300)] public string? ImageURL { get; set; }
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public string? Biography { get; set; }
        [Range(0, 80)] public int? ExperienceYears { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal ConsultationFees { get; set; }
        public bool? Available { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
