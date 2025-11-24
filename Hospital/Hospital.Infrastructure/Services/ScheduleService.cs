using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
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
            _logger.LogInformation("Creating schedule for DoctorId: {DoctorId}, Day: {DayOfWeek}, Shift: {Shift}", dto.DoctorId, dto.DayOfWeek, dto.AppointmentShift);

            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", dto.DoctorId);
                throw new KeyNotFoundException("Doctor not found.");
            }

            if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek, true, out _))
                throw new ArgumentException("Invalid day of the week.");

            if (!ShiftTimeRanges.Shifts.ContainsKey(dto.AppointmentShift))
                throw new ArgumentException("Invalid appointment shift.");

            var existingSchedules = await _repo.GetAllByDayOfWeekAsync(dto.DayOfWeek);
            if (existingSchedules.Any(s => s.DoctorId == dto.DoctorId && s.Shift == dto.AppointmentShift))
            {
                _logger.LogWarning("Duplicate schedule detected for DoctorId {DoctorId} on {DayOfWeek} shift {Shift}", dto.DoctorId, dto.DayOfWeek, dto.AppointmentShift);
                throw new InvalidOperationException("Doctor already has a schedule for this day and shift.");
            }

            var schedule = _mapper.Map<Schedule>(dto);
            var timeRange = ShiftTimeRanges.Shifts[dto.AppointmentShift];
            schedule.Shift = dto.AppointmentShift;
            schedule.StartTime = DateTime.Today.Add(timeRange.Start);
            schedule.EndTime = DateTime.Today.Add(timeRange.End);
            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _repo.AddAsync(schedule);
            _logger.LogInformation("Schedule created successfully for DoctorId {DoctorId}", dto.DoctorId);

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> UpdateAsync(UpdateScheduleDto dto)
        {
            _logger.LogInformation("Updating schedule with ScheduleId: {ScheduleId}", dto.ScheduleId);

            var schedule = await _repo.GetByIdAsync(dto.ScheduleId);
            if (schedule == null)
            {
                _logger.LogWarning("Schedule with ID {ScheduleId} not found", dto.ScheduleId);
                throw new KeyNotFoundException("Schedule not found.");
            }

            if (dto.AppointmentShift.HasValue)
            {
                if (!ShiftTimeRanges.Shifts.ContainsKey(dto.AppointmentShift.Value))
                    throw new ArgumentException("Invalid appointment shift.");

                var range = ShiftTimeRanges.Shifts[dto.AppointmentShift.Value];
                schedule.StartTime = DateTime.Today.Add(range.Start);
                schedule.EndTime = DateTime.Today.Add(range.End);
                schedule.Shift = dto.AppointmentShift.Value;

                var otherSchedules = await _repo.GetAllByDayOfWeekAsync(schedule.DayOfWeek);
                if (otherSchedules.Any(s => s.DoctorId == schedule.DoctorId && s.Shift == schedule.Shift && s.ScheduleId != schedule.ScheduleId))
                {
                    _logger.LogWarning("Duplicate schedule conflict for DoctorId {DoctorId} on {DayOfWeek} shift {Shift}", schedule.DoctorId, schedule.DayOfWeek, schedule.Shift);
                    throw new InvalidOperationException("Doctor already has a schedule for this day and shift.");
                }
            }

            _mapper.Map(dto, schedule);
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            _logger.LogInformation("Schedule with ScheduleId {ScheduleId} updated successfully", dto.ScheduleId);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<bool> DeleteAsync(int scheduleId)
        {
            _logger.LogInformation("Deleting schedule with ScheduleId: {ScheduleId}", scheduleId);
            var deleted = await _repo.DeleteAsync(scheduleId);
            _logger.LogInformation(deleted > 0 ? "Schedule deleted successfully" : "Failed to delete schedule");
            return deleted > 0;
        }

        public async Task<ScheduleDto?> GetByIdAsync(int scheduleId)
        {
            _logger.LogInformation("Fetching schedule with ScheduleId: {ScheduleId}", scheduleId);
            var schedule = await _repo.GetByIdAsync(scheduleId);
            return schedule == null ? null : _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all schedules");
            var schedules = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAsync(string dayOfWeek)
        {
            _logger.LogInformation("Fetching schedules for DayOfWeek: {DayOfWeek}", dayOfWeek);
            if (!Enum.TryParse<DayOfWeek>(dayOfWeek, true, out _))
                throw new ArgumentException("Invalid day of the week.");

            var schedules = await _repo.GetAllByDayOfWeekAsync(dayOfWeek);
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetDoctorsByDateAndShiftAsync(string dayOfWeek, AppointmentShift shift)
        {
            _logger.LogInformation("Fetching schedules for DayOfWeek: {DayOfWeek} and Shift: {Shift}", dayOfWeek, shift);

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
            _logger.LogInformation("Fetching schedules for DoctorId: {DoctorId}", doctorId);
            var schedules = await _repo.GetAllByDoctorIdAsync(doctorId);
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }
    }
}