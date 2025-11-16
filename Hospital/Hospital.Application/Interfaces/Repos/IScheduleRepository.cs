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
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task<IEnumerable<Schedule>> GetAllByDoctorIdAsync(int id);
        Task AddAsync(Schedule schedule);
        Task UpdateAsync(Schedule schedule);
        Task DeleteAsync(int id);

        Task<bool> DoctorHasScheduleAsync(int doctorId, string dayOfWeek);
    }
}
