using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repos
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;
        public DoctorRepository(AppDbContext context) 
        {
            _context = context; 
        }

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            doctor.CreatedAt = DateTime.UtcNow;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<int> UpdateAsync(Doctor doctor)
        {
            doctor.UpdatedAt = DateTime.UtcNow;
            _context.Doctors.Update(doctor);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return 0;
            _context.Doctors.Remove(doctor);
            return await _context.SaveChangesAsync();
        }

        public async Task<Doctor?> GetAsync(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .Include(d => d.Branches)
                 .Include(d => d.Schedules)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<IEnumerable<Doctor>> GetAllByBranchAsync(int branchId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .Include(d => d.Branches)
                 .Include(d => d.Schedules)
                .Where(d => d.Branches.Any(b => b.BranchId == branchId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .Include(d => d.Branches)
                .Include(d => d.Schedules)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationIdAsync(int specializationId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Branches)
                    .ThenInclude(b => b.Specializations)
                .Include(d => d.Schedules)
                .Where(d => d.Branches
                    .Any(b => b.Specializations
                        .Any(s => s.SpecializationId == specializationId)))
                .ToListAsync();
        }
       
       public async Task<List<Appointment>> GetAppoimentsByDateForDoctorAsync(int doctorId, DateOnly date)
{
    return await _context.Appointments
        .Include(a => a.Patient)
            .ThenInclude(p => p.User)
        .Include(a => a.Branch)
        .Where(a =>
            a.DoctorId == doctorId &&
            a.Date.Day == date.Day &&
            a.Date.Month == date.Month &&
            a.Date.Year == date.Year &&
            (a.Status == AppointmentStatus.Confirmed ||
             a.Status == AppointmentStatus.Completed))
        .ToListAsync();
}


    }
}
