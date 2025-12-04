using Hospital.Application.DTO.Branch;
using Hospital.Application.DTO.Doctor;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Specialization
{
    public class SpecializationDTO
    {
        [Key] public int SpecializationId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } 
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<DoctorMiniDto> Doctors { get; set; } = new List<DoctorMiniDto>();
        public ICollection<BranchMiniDto> Branches { get; set; } = new List<BranchMiniDto>();
    }
}
