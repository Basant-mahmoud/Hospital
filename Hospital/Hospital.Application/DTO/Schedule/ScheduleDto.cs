using Hospital.Domain.Enum;
using System;

namespace Hospital.Application.DTO.Schedule
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }

        // Schedule Info
        public string DayOfWeek { get; set; } = null!;
        public AppointmentShift AppointmentShift { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Doctor Info
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string? ImageURL { get; set; }
        public decimal? ConsultationFees { get; set; }
        public bool Available { get; set; }

        // Specialization Info
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; } = null!;

        // Branch Info
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
    }
}
