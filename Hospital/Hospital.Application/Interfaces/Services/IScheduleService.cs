using Hospital.Application.DTO.Schedule;
using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public interface IScheduleService
    {
        Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
        Task<ScheduleDto> UpdateAsync(UpdateScheduleDto dto);
        Task<bool> DeleteAsync(int scheduleId);
        Task<ScheduleDto?> GetByIdAsync(int scheduleId);
        Task<IEnumerable<ScheduleDto>> GetAllAsync();
        Task<IEnumerable<ScheduleDto>> GetSchedulesByDoctorIdAsync(int doctorId);
        Task<IEnumerable<ScheduleDto>> GetSchedulesByDateOnlyAsync(DateOnly date);
        Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAndShiftAsync( DateOnly date,AppointmentShift shift);

    }
}
