using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Patient
{
     public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
    }
}
