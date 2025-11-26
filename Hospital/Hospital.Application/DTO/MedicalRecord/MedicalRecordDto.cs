using Hospital.Application.DTO.Appointment;
using Hospital.Application.DTO.Doctor;
using Hospital.Application.DTO.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.MedicalRecord
{
    public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DoctorMedicalDto Doctor { get; set; }
        public PatientMedicalRecordDto Patient { get; set; }
        public AppointmentDto Appointment { get; set; }
    }
}
