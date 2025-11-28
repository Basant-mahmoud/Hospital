using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Branch
{
    public class UpdateBranchDto
    {

        [Required]
        public int BranchId { get; set; }

        [Required, StringLength(150)]
        public string BranchName { get; set; } = null!;

        [Required, StringLength(250)]
        public string Address { get; set; } = null!;

        [Phone, StringLength(25)]
        public string? Phone { get; set; }

        [EmailAddress, StringLength(200)]
        public string? Email { get; set; }

        public string? Description { get; set; }
        [Url, StringLength(300)] public string? ImageURL { get; set; }
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double? Longitude { get; set; }

    }
}
