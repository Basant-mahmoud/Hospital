using Hospital.Application.DTO.Doctor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Specialization
{
    public class SpecializationInfoDto
    {
        [Key] public int SpecializationId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<BranchMiniDto> Branches { get; set; } = new();
        public ICollection<DoctorMiniDto> Doctors { get; set; } = new List<DoctorMiniDto>();
    }
}
