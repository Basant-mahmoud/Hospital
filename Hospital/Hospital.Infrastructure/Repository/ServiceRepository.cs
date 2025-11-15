using AutoMapper;
using Clinic.Infrastructure.Persistence;
using Hospital.Application.DTO;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Hospital.Infrastructure.Repository
{
    public class ServiceRepository : IServiceRepository

    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
          
            return await _context.Services
            .Include(s => s.Branches) 
            .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services
            .Include(s => s.Branches) 
            .FirstOrDefaultAsync(s => s.ServiceId == id);
        }

        public async Task<Service> AddAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsByNameInBranchesAsync(string name, IEnumerable<int> branchIds)
        {
            return await _context.Services
                .Where(s => s.Name.ToLower() == name.ToLower())
                .AnyAsync(s => s.Branches.Any(b => branchIds.Contains(b.BranchId)));
        }

    }
}



