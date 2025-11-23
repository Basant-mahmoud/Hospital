using Hospital.Domain.Models;
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
        Task<bool> ExistsAsync(int doctorId, DateOnly date, DateTime time);
        //Task<bool > convertStatuesOFPaymentToPayied(Hospital.Domain.Models.Payment payment);

    }
}
