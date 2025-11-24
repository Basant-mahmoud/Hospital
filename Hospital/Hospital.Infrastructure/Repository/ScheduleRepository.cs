using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repository
{
    public class ScheduleRepository : IScheduleRepository
    {

        private readonly AppDbContext _context;
        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> AddAsync(Schedule schedule)
        {
            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<int> UpdateAsync(Schedule schedule)
        {
            schedule.UpdatedAt = DateTime.UtcNow;
            _context.Schedules.Update(schedule);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null) return 0;
            _context.Schedules.Remove(schedule);
            return await _context.SaveChangesAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int scheduleId)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                 .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                 .ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetAllByDoctorIdAsync(int doctorId)
        {
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId)
                .Include(s => s.Doctor)
                 .ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetAllByDayOfWeekAsync(string dayOfWeek)
        {
            return await _context.Schedules
                .Where(s => s.DayOfWeek.ToLower() == dayOfWeek.ToLower())
                .Include(s => s.Doctor)
                 .ThenInclude(d => d.User)
                .ToListAsync();
        }

    }
}