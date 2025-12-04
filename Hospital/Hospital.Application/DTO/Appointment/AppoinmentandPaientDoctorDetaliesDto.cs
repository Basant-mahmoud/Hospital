using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Appointment
{
    public class AppoinmentandPaientDoctorDetaliesDto
    {

        public AppointmentDto appointmentDetails { get; set; }
        public PatientAppointmentDto patientInfo { get; set; }
        public appointmentDoctorDto doctorinfo { get; set; }
    }
}
