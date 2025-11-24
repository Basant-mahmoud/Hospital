using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Appointment
{
    public class AddAppointmentDto
    {
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        [Required] public int BranchId { get; set; }
        [Required] public DateOnly Date { get; set; }
        [Required] public AppointmentShift Shift { get; set; }
        [Required] public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
        [Required] public PaymentMethod PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}
