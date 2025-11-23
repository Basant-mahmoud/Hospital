using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface IDoctorRepository
    {
        Task<Doctor> AddAsync(Doctor doctor);
        Task<int> UpdateAsync(Doctor doctor);
        Task<int> DeleteAsync(int doctorId);
        Task<Doctor?> GetAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetAllByBranchAsync(int branchId);
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationIdAsync(int specializationId);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<List<Appointment>> GetAppoimentsByDateForDoctorAsync(int doctorId, DateOnly date);
    }
}
