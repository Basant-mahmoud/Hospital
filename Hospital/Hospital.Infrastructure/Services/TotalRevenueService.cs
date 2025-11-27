using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class TotalRevenueService : ITotalRevenueService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IBranchRepository _branchRepository;

        public TotalRevenueService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IBranchRepository branchRepository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _branchRepository = branchRepository;
        }

        public async Task<decimal> GetTotalRevenueByDoctorIdInSpecificBranchAsync(int doctorId, int branchId)
        {
            if (doctorId < 1) throw new KeyNotFoundException("Doctor id not found");
            if (branchId < 1) throw new KeyNotFoundException("Branch id not found");

            var doctor = await _doctorRepository.GetAsync(doctorId);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found");

            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) throw new KeyNotFoundException("Branch not found");

            var appointments = (await _appointmentRepository.GetByDoctorIdAsync(doctorId))
                .Where(a => a.BranchId == branchId && a.Status == AppointmentStatus.Completed);

            var totalRevenue = appointments.Count() * (doctor.ConsultationFees );
            return totalRevenue;
        }

        public async Task<decimal> GetTotalRevenueByDoctorIdInAllBranchAsync(int doctorId)
        {
            if (doctorId < 1) throw new KeyNotFoundException("Doctor id not found");

            var doctor = await _doctorRepository.GetAsync(doctorId);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found");

            var appointments = (await _appointmentRepository.GetByDoctorIdAsync(doctorId))
                .Where(a => a.Status == AppointmentStatus.Completed);

            var totalRevenue = appointments.Count() * (doctor.ConsultationFees );
            return totalRevenue;
        }

        // 3. Total revenue in all branches (all doctors)
        public async Task<decimal> GetTotalRevenueInAllBranchAsync()
        {
            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a => a.Status == AppointmentStatus.Completed);

            decimal totalRevenue = 0;
            foreach (var appointment in appointments)
            {
                if (appointment.Doctor != null)
                    totalRevenue += appointment.Doctor.ConsultationFees ;
            }

            return totalRevenue;
        }

        // 4. Total revenue in a specific branch (all doctors)
        public async Task<decimal> GetTotalRevenueInSpecificBranchAllBranchAsync(int branchId)
        {
            if (branchId < 1) throw new KeyNotFoundException("Branch id not found");
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) throw new KeyNotFoundException("Branch not found");

            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a => a.BranchId == branchId && a.Status == AppointmentStatus.Completed);

            decimal totalRevenue = 0;
            foreach (var appointment in appointments)
            {
                if (appointment.Doctor != null)
                    totalRevenue += appointment.Doctor.ConsultationFees ;
            }

            return totalRevenue;
        }
    }

}
