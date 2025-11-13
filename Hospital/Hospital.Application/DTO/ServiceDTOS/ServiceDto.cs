using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.ServiceDTOS
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public ICollection<BranchIdDTO>? BranchesID { get; set; } = new List<BranchIdDTO>();
    }
}
