using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Appointment
{
    public class appointmentDoctorDto
    {
        [Key] public int DoctorId { get; set; }
        [Required] public int SpecializationId { get; set; }
    }
}
