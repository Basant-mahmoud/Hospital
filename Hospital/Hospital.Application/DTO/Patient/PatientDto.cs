using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Patient
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public List<MedicalRecordDto> MedicalRecords { get; set; } = new();
    }
}
