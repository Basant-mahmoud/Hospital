using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto?> GetByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            return schedule == null ? null : _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllByDoctorIdAsync(int doctorId)
        {
            var schedules = await _scheduleRepository.GetAllByDoctorIdAsync(doctorId);
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
        {
            // Validate doctor exists
            var doctor = await _doctorRepository.GetAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            // Check if doctor already has schedule on the same day
            bool exists = await _scheduleRepository.DoctorHasScheduleAsync(dto.DoctorId, dto.DayOfWeek);
            if (exists)
                throw new InvalidOperationException("Doctor already has a schedule for this day.");

            // Validate time
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("EndTime must be greater than StartTime.");

            if (!Enum.TryParse<System.DayOfWeek>(dto.DayOfWeek, true, out var day))
                throw new ArgumentException("Invalid DayOfWeek. Must be Sunday, Monday, Tuesday, etc.");

       

            var schedule = _mapper.Map<Schedule>(dto);
            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _scheduleRepository.AddAsync(schedule);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task UpdateAsync(UpdateScheduleDto dto)
        {
            var existing = await _scheduleRepository.GetByIdAsync(dto.ScheduleId);
            if (existing == null)
                throw new KeyNotFoundException("Schedule not found.");

            // If day or doctor changed, revalidate uniqueness
            if (dto.DayOfWeek != existing.DayOfWeek || dto.DoctorId != existing.DoctorId)
            {
                bool exists = await _scheduleRepository.DoctorHasScheduleAsync(dto.DoctorId, dto.DayOfWeek);
                if (exists)
                    throw new InvalidOperationException("Doctor already has a schedule on this day.");
            }

            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("EndTime must be greater than StartTime.");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _scheduleRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null)
                return false;

            await _scheduleRepository.DeleteAsync(id);
            return true;
        }
    }
}
