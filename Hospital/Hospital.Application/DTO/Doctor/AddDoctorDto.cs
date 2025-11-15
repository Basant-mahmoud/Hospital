using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Doctor
{
    public class AddDoctorDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        // doctor specific
        public int SpecializationId { get; set; }
        public List<int> BranchIds { get; set; } = new();
        public string? Biography { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFees { get; set; }
        public bool? Available { get; set; }
    }
}
