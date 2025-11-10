using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class User
    {
        [Key] public int UserId { get; set; }
        [Required, StringLength(150)] public string FullName { get; set; } = null!;
        [Required, EmailAddress, StringLength(200)] public string Email { get; set; } = null!;
        [Required, StringLength(255)] public string PasswordHash { get; set; } = null!;
        [Phone, StringLength(25)] public string? Phone { get; set; }
        public GenderType? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        [Required, StringLength(50)] public string Role { get; set; } = "User";
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
        public Doctor? DoctorProfile { get; set; }
        public Patient? PatientProfile { get; set; }
        public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
    }
}
