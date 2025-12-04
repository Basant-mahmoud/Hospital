using Hospital.Application.DTO.Specialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Appointment
{

    public class DoctorShortDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } 
        public string? ImageURL { get; set; }
        public SpecializationDto Specialization { get; set; }
    }
    public class SpecializationDto
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; }
    }

}
    
