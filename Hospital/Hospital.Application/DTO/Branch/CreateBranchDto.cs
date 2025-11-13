using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Branch
{
    public class CreateBranchDto
    {
        [Required, StringLength(150)]
        public string BranchName { get; set; } = null!;

        [StringLength(250)]
        public string Address { get; set; }

        [Phone, StringLength(25)]
        public string? Phone { get; set; }

        [EmailAddress, StringLength(200)]
        public string? Email { get; set; }

        public string? Description { get; set; }
    }
}
