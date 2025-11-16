using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Patient
{
    public class UpdatePatientDto
    {
        [Required] public int PatientId { get; set; }
        [Required] public string FullName { get; set; } = null!;
        [Required] public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; } = null!;
    }
}
