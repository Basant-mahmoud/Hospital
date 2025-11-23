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
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _dbContext;

        public PatientRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Patient patient)
        {
            _dbContext.Patients.Add(patient);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _dbContext.Patients
                .Include(p => p.User)
                .Include(p => p.MedicalRecords)
                    .ThenInclude(m => m.Doctor)
                        .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }


        public async Task<List<Patient>> GetAllAsync()
        {
            return await _dbContext.Patients.Include(p => p.User)
                .Include(p => p.MedicalRecords)
                    .ThenInclude(m => m.Doctor)
                        .ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            _dbContext.Patients.Update(patient);
            await _dbContext.SaveChangesAsync();
        }
    }
}

