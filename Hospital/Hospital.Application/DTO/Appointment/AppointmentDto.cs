using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Appointment
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateOnly Date { get; set; }
        public AppointmentShift Shift { get; set; }
        public string Status { get; set; }
        public BranchShortDto Branch { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentDto Payment { get; set; }
        public DoctorShortDto Doctor { get; set; }


    }
    public class BranchShortDto
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
    }
}
