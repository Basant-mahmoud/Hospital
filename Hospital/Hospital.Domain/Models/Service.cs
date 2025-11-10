using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class Service
    {
        [Key] public int ServiceId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Url, StringLength(300)] public string? ImageURL { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int? SpecializationId { get; set; }
        public Specialization? Specialization { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
