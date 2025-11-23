using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Specialization
{
    public class CreateSpecialization
    {
        [Required, StringLength(120)] public string Name { get; set; } 
        public string? Description { get; set; }
        public List<int> BranchIds { get; set; } = new List<int>();
    }
}
