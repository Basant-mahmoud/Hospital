using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IMapper _mapper;

        public ScheduleService(IScheduleRepository repo, IMapper mapper, IDoctorRepository doctorRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _doctorRepo = doctorRepo;
        }

        public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
        {
            // ✅ Validate Doctor exists
            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            // ✅ Validate DayOfWeek
            if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek, true, out _))
                throw new ArgumentException("Invalid day of the week. Must be Sunday, Monday, etc.");

            // ✅ Validate AppointmentShift
            if (!ShiftTimeRanges.Shifts.ContainsKey(dto.AppointmentShift))
                throw new ArgumentException("Invalid appointment shift.");

            // ✅ Prevent duplicate schedule for same doctor/day/shift
            var existingSchedules = await _repo.GetAllByDayOfWeekAsync(dto.DayOfWeek);
            if (existingSchedules.Any(s => s.DoctorId == dto.DoctorId && s.Shift == dto.AppointmentShift))
                throw new InvalidOperationException("Doctor already has a schedule for this day and shift.");

            // Map and compute times
            var schedule = _mapper.Map<Schedule>(dto);
            var timeRange = ShiftTimeRanges.Shifts[dto.AppointmentShift];
            schedule.Shift = dto.AppointmentShift;
            schedule.StartTime = DateTime.Today.Add(timeRange.Start);
            schedule.EndTime = DateTime.Today.Add(timeRange.End);
            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _repo.AddAsync(schedule);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> UpdateAsync(UpdateScheduleDto dto)
        {
            var schedule = await _repo.GetByIdAsync(dto.ScheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            // Validate optional shift
            if (dto.AppointmentShift.HasValue)
            {
                if (!ShiftTimeRanges.Shifts.ContainsKey(dto.AppointmentShift.Value))
                    throw new ArgumentException("Invalid appointment shift.");

                var range = ShiftTimeRanges.Shifts[dto.AppointmentShift.Value];
                schedule.StartTime = DateTime.Today.Add(range.Start);
                schedule.EndTime = DateTime.Today.Add(range.End);
                schedule.Shift = dto.AppointmentShift.Value;

                // Prevent duplicates
                var otherSchedules = await _repo.GetAllByDayOfWeekAsync(schedule.DayOfWeek);
                if (otherSchedules.Any(s => s.DoctorId == schedule.DoctorId && s.Shift == schedule.Shift && s.ScheduleId != schedule.ScheduleId))
                    throw new InvalidOperationException("Doctor already has a schedule for this day and shift.");
            }

            _mapper.Map(dto, schedule);
            schedule.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(schedule);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<bool> DeleteAsync(int scheduleId)
        {
            var deleted = await _repo.DeleteAsync(scheduleId);
            return deleted > 0;
        }

        public async Task<ScheduleDto?> GetByIdAsync(int scheduleId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId);
            return schedule == null ? null : _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            var schedules = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAsync(string dayOfWeek)
        {
            if (!Enum.TryParse<DayOfWeek>(dayOfWeek, true, out _))
                throw new ArgumentException("Invalid day of the week. Must be Sunday, Monday, etc.");

            var schedules = await _repo.GetAllByDayOfWeekAsync(dayOfWeek);
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAndShiftAsync(string dayOfWeek, AppointmentShift shift)
        {
            if (!Enum.IsDefined(typeof(AppointmentShift), shift))
                throw new ArgumentException("Invalid appointment shift.");

            if (!Enum.TryParse<DayOfWeek>(dayOfWeek, true, out _))
                throw new ArgumentException("Invalid day of the week.");

            var schedules = await _repo.GetAllByDayOfWeekAsync(dayOfWeek);
            var range = ShiftTimeRanges.Shifts[shift];

            var filtered = schedules
                .Where(s => s.StartTime.TimeOfDay >= range.Start && s.EndTime.TimeOfDay <= range.End)
                .ToList();

            return _mapper.Map<IEnumerable<ScheduleDto>>(filtered);
        }
        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByDoctorIdAsync(int doctorId)
        {
            var schedules = await _repo.GetAllByDoctorIdAsync(doctorId);
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }
    }
}
