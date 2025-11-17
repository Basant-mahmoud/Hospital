using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Doctor
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }

        public int SpecializationId { get; set; }
        public string? SpecializationName { get; set; }
        public string? ImageURL { get; set; }

        public List<int> BranchID { get; set; } = new();
        public string? Biography { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFees { get; set; }
        public bool? Available { get; set; }
        public ICollection<DoctorSchuduleDto> Schedules { get; set; } = new List<DoctorSchuduleDto>();


    }
}
