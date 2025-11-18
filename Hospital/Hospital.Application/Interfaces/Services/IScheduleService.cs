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
        Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
        Task<ScheduleDto> UpdateAsync(UpdateScheduleDto dto);
        Task<bool> DeleteAsync(int scheduleId);
        Task<ScheduleDto?> GetByIdAsync(int scheduleId);
        Task<IEnumerable<ScheduleDto>> GetAllAsync();
        Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAsync(string dayOfWeek);
        Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAndShiftAsync(string dayOfWeek, Domain.Enum.AppointmentShift shift);
        Task<IEnumerable<ScheduleDto>> GetSchedulesByDoctorIdAsync(int doctorId);

    }
}
