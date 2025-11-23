using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.ServiceDTOS
{
    public class CreateServiceDto
    {

        [Required, StringLength(120)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Url, StringLength(300)]
        public string? ImageURL { get; set; }
        public ICollection<BranchIdDTO>? BranchesID { get; set; } = new List<BranchIdDTO>();
    }
}
