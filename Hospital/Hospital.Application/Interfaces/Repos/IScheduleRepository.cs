using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface IScheduleRepository
    {
        Task<Schedule> AddAsync(Schedule schedule);
        Task<int> UpdateAsync(Schedule schedule);
        Task<int> DeleteAsync(int scheduleId);
        Task<Schedule?> GetByIdAsync(int scheduleId);
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<IEnumerable<Schedule>> GetAllByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Schedule>> GetAllByDateAsync(DateOnly date);
    }
}
