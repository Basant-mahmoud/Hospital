using Clinic.Infrastructure.Persistence;
using Hospital.Application.DTO.Specialization;
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
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly AppDbContext _dbContext;
        public SpecializationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Specialization> AddAsync(Specialization specialization)
        {
            await _dbContext.Specializations.AddAsync(specialization);
            await _dbContext.SaveChangesAsync();
            return specialization;
        }

        public async Task<int> DeleteAsync(Specialization specialization)
        {
            _dbContext.Specializations.Remove(specialization);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Specialization>> GetAllByBranchAsync(int branchId)
        {
            var specializations = await _dbContext.Specializations
                .Include(s => s.Branches)
                .Include(s => s.Doctors)
                .ThenInclude(d => d.User)
                .Where(s => s.Branches.Any(b => b.BranchId == branchId))
                .ToListAsync();

            return specializations;
        }


        public async Task<IEnumerable<Specialization>> GetAllSpecializationInSystemAsync()
        {
            return await _dbContext.Specializations
                 .Include(s => s.Branches)         
                 .Include(s => s.Doctors)           
                     .ThenInclude(d => d.User)       
                 .ToListAsync();
        }

        public async Task<Specialization?> GetAsync(int id)
        {
            return await _dbContext.Specializations
                  .Include(s => s.Branches)
                  .Include(s => s.Doctors)   
                  .ThenInclude(d => d.User)  
                  .FirstOrDefaultAsync(s => s.SpecializationId == id);
        }

        public async Task<int> UpdateAsync(Specialization specialization)
        {
            _dbContext.Specializations.Update(specialization);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
