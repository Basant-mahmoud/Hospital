using Hospital.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class User: IdentityUser
    {
        [Required, StringLength(150)] public string FullName { get; set; } = null!;
        public GenderType? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        [Required, StringLength(50)] public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
        public Doctor? DoctorProfile { get; set; }
        public Patient? PatientProfile { get; set; }

        public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
        public ICollection<PasswordResetCode> PasswordResetCodes { get; set; } = new List<PasswordResetCode>();

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
