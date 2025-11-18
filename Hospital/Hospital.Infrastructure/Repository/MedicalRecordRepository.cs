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
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly AppDbContext _dbContext;

        public MedicalRecordRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<MedicalRecord> AddAsync(MedicalRecord medical)
        {
            await _dbContext.MedicalRecords.AddAsync(medical);
            await _dbContext.SaveChangesAsync();
            return medical;
        }

        public async Task<int> UpdateAsync(MedicalRecord medical)
        {
            _dbContext.MedicalRecords.Update(medical);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(MedicalRecord medical)
        {
            _dbContext.MedicalRecords.Remove(medical);
            return await _dbContext.SaveChangesAsync();
        }

        //public async Task<IEnumerable<MedicalRecord>> GetAllAsync(int branchId)
        //{
        //    return await _dbContext.MedicalRecords
        //        .Include(m => m.Doctor)
        //        .Include(m => m.Patient)
        //        .Include(m => m.Appointment)
        //        .Where(m => m.Doctor.DoctorId == branchId) 
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<MedicalRecord>> GetAllEventInSystemAsync()
        {
            return await _dbContext.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Appointment)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetAsync(int id)
        {
            return await _dbContext.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Appointment)
                .FirstOrDefaultAsync(m => m.RecordId == id);
        }
        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbContext.MedicalRecords
                .Include(m => m.Patient) // جلب بيانات المريض
                .Include(m => m.Doctor)  // جلب بيانات الدكتور
                .Include(m => m.Appointment)
                .Where(m => m.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _dbContext.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }
    }
}
