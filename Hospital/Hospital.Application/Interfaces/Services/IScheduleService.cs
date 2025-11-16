using Hospital.Application.DTO.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllAsync();
        Task<ScheduleDto?> GetByIdAsync(int id);
        Task<IEnumerable<ScheduleDto>> GetAllByDoctorIdAsync(int doctorId);

        Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
        Task UpdateAsync(UpdateScheduleDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
