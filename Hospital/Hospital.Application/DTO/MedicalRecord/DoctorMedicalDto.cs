using Hospital.Application.DTO.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.MedicalRecord
{
    public class DoctorMedicalDto
    {
        public int DoctorId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int SpecializationId { get; set; }
        public string? SpecializationName { get; set; }
        public string? ImageURL { get; set; }
        public string? Biography { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFees { get; set; }
        public bool? Available { get; set; }
    }
}
