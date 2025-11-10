using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class Branch
    {
        [Key] public int BranchId { get; set; }
        [Required, StringLength(150)] public string BranchName { get; set; } = null!;
        [StringLength(250)] public string? Address { get; set; }
        [Phone, StringLength(25)] public string? Phone { get; set; }
        [EmailAddress, StringLength(200)] public string? Email { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<News> News { get; set; } = new List<News>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Banner> Banners { get; set; } = new List<Banner>();
    }
}
