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

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DoctorHasScheduleAsync(int doctorId, string dayOfWeek)
        {
            return await _context.Schedules
                .AnyAsync(s => s.DoctorId == doctorId &&
                               s.DayOfWeek.ToLower() == dayOfWeek.ToLower());
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetAllByDoctorIdAsync(int doctorId)
        {
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId)
                .Include(s => s.Doctor)
                .ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }
}
