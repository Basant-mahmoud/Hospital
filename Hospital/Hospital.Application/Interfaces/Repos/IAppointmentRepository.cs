using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface IAppointmentRepository
    {
        Task<Appointment> AddAsync(Appointment appointment);
        Task<int> DeleteAsync(Appointment appointment);
        Task<Appointment?> GetAsync(int id);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> GetCompletedAppointmentAsync(int doctorid);
        Task<int> UpdateAsync(Appointment appointment);
        Task<int> UpdateRangeAsync(IEnumerable<Appointment> appointments);
        Task<bool> PatientBookedSameDoctorSameShiftAsync(int patientId, int doctorId, DateOnly date, AppointmentShift shift);
        Task<IEnumerable<Appointment>> GetCancelByDoctorIdAsync(int doctorId);
    }
}
