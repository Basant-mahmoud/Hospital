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
    public class BranchRepository : IBranchRepository
    {
        private readonly AppDbContext _context;
        public BranchRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Branch branch)
        {
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            return await _context.Branches.AsNoTracking().ToListAsync();
        }

        public async Task<Branch?> GetByIdAsync(int id)
        {
            return await _context.Branches
                .FirstOrDefaultAsync(e => e.BranchId == id);
        }

        public async Task UpdateAsync(Branch branch)
        {
            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Branch>> GetBranchesByIdsAsync(List<int> branchIds)
        {
            return await _context.Branches
                .Where(b => branchIds.Contains(b.BranchId))
                .ToListAsync();
        }
    }
}
