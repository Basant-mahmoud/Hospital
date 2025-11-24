using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IScheduleRepository repo, IMapper mapper, IDoctorRepository doctorRepo, ILogger<ScheduleService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _doctorRepo = doctorRepo;
            _logger = logger;
        }

        public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
        {
            _logger.LogInformation(
                "Creating schedule for DoctorId: {DoctorId}, Date: {Date}, Shift: {Shift}",
                dto.DoctorId, dto.ScheduleDate, dto.AppointmentShift);

            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            // Validate shift exists
            if (!ShiftTimeRanges.Shifts.ContainsKey(dto.AppointmentShift))
                throw new ArgumentException("Invalid appointment shift.");

            // Check for existing schedule on same date and shift
            var existingSchedules = await _repo.GetAllByDateAsync(dto.ScheduleDate);
            if (existingSchedules.Any(s => s.DoctorId == dto.DoctorId && s.Shift == dto.AppointmentShift))
                throw new ValidationException("Doctor already has a schedule for this date and shift.");

            // Mapping
            var schedule = _mapper.Map<Schedule>(dto);

            var timeRange = ShiftTimeRanges.Shifts[dto.AppointmentShift];
            var scheduleDateTime = dto.ScheduleDate.ToDateTime(TimeOnly.MinValue);

            schedule.StartTime = scheduleDateTime.Add(timeRange.Start);
            schedule.EndTime = scheduleDateTime.Add(timeRange.End);
            schedule.Shift = dto.AppointmentShift;
            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _repo.AddAsync(schedule);

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> UpdateAsync(UpdateScheduleDto dto)
        {
            _logger.LogInformation("Updating schedule with ScheduleId: {ScheduleId}", dto.ScheduleId);

            var schedule = await _repo.GetByIdAsync(dto.ScheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            // Update date if changed
            if (dto.ScheduleDate != default)
            {
                schedule.ScheduleDate = dto.ScheduleDate;
            }

            // Update shift if provided
            if (dto.AppointmentShift.HasValue)
            {
                if (!ShiftTimeRanges.Shifts.ContainsKey(dto.AppointmentShift.Value))
                    throw new ArgumentException("Invalid appointment shift.");

                var range = ShiftTimeRanges.Shifts[dto.AppointmentShift.Value];
                var scheduleDateTime = schedule.ScheduleDate.ToDateTime(TimeOnly.MinValue);

                schedule.StartTime = scheduleDateTime.Add(range.Start);
                schedule.EndTime = scheduleDateTime.Add(range.End);
                schedule.Shift = dto.AppointmentShift.Value;

                // Check for conflicts
                var otherSchedules = await _repo.GetAllByDateAsync(schedule.ScheduleDate);
                if (otherSchedules.Any(s => s.DoctorId == schedule.DoctorId && s.Shift == schedule.Shift && s.ScheduleId != schedule.ScheduleId))
                    throw new InvalidOperationException("Doctor already has a schedule for this date and shift.");
            }

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
                throw new ArgumentException("Invalid day of the week.");

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
        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByDateOnlyAsync(DateOnly date)
        {
            _logger.LogInformation("Fetching schedules for date {Date} at {Time}", date, DateTime.UtcNow);

            // Validate date is not in the past
            if (date < DateOnly.FromDateTime(DateTime.Today))
            {
                _logger.LogWarning("Attempted to fetch schedules for a past date: {Date}", date);
                throw new ArgumentException("Cannot get schedules for a past date.");
            }

            // Fetch schedules from repository
            var schedules = await _repo.GetAllByDateAsync(date);

            if (schedules == null || !schedules.Any())
            {
                _logger.LogInformation("No schedules found for date {Date}", date);
                return new List<ScheduleDto>();
            }

            _logger.LogInformation("Found {Count} schedules for date {Date}", schedules.Count(), date);

            // Map to DTO
            var result = _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
            return result;
        }

    }
}
