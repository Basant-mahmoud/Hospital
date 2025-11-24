using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<int> DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return await _context.SaveChangesAsync();
        }

        public async Task<Appointment?> GetAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }
        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient)
                .Include(a => a.Branch)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .ToListAsync();
        }

        public async Task<int> UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<Appointment> appointments)
        {
            _context.Appointments.UpdateRange(appointments);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> PatientBookedSameDoctorSameShiftAsync(int patientId, int doctorId, DateOnly date, AppointmentShift shift)
        {
            return await _context.Appointments
                .AnyAsync(a =>
                    a.PatientId == patientId &&
                    a.DoctorId == doctorId &&
                    a.Date == date &&
                    a.Shift == shift
                );
        }

    }
}
