using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _dbContext;

        public EventRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Event> AddAsync(Event @event)
        {
           _dbContext.Events.Add(@event);
            await _dbContext.SaveChangesAsync();
            return @event;

        }

        public async Task<int> DeleteAsync(Event @event)
        {
            _dbContext.Events.Remove(@event);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Event>> GetAllAsync( int branchId)
        {
            return await _dbContext.Events
                  .AsNoTracking()
                .Where(b=>b.BranchId== branchId).ToListAsync();
        }

        public async Task<Event?> GetAsync(int id)
        {
            return await _dbContext.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<int> UpdateAsync(Event @event)
        {
            _dbContext.Events.Update(@event);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
