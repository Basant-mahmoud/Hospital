using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.MedicalRecord
{
    public class PatientMedicalRecordDto
    {
        public int RecordId { get; set; }
        public int AppointmentId { get; set; }

        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public int PatientId { get; set; }
        public string? PatientName { get; set; }

        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
