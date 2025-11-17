using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Doctor
{
    public class DoctorSelfUpdateDto
    {
        public int DoctorId { get; set; }

        public string FullName { get; set; }
        public string? Biography { get; set; }

        public int? ExperienceYears { get; set; }

        public decimal? ConsultationFees { get; set; }

        public bool? Available { get; set; }

        public string? PhoneNumber { get; set; }

        public int SpecializationId { get; set; }

        public List<int> BranchIds { get; set; } = new();
    }


}
