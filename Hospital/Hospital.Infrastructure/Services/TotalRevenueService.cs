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

            var totalRevenue = appointments.Count() * (doctor.ConsultationFees);
            return totalRevenue;
        }

        public async Task<decimal> GetTotalRevenueByDoctorIdInAllBranchAsync(int doctorId)
        {
            if (doctorId < 1) throw new KeyNotFoundException("Doctor id not found");

            var doctor = await _doctorRepository.GetAsync(doctorId);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found");

            var appointments = (await _appointmentRepository.GetByDoctorIdAsync(doctorId))
                .Where(a => a.Status == AppointmentStatus.Completed);

            var totalRevenue = appointments.Count() * (doctor.ConsultationFees);
            return totalRevenue;
        }

        public async Task<decimal> GetTotalRevenueInAllBranchAsync()
        {
            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a => a.Status == AppointmentStatus.Completed);

            decimal totalRevenue = 0;
            foreach (var appointment in appointments)
            {
                if (appointment.Doctor != null)
                    totalRevenue += appointment.Doctor.ConsultationFees;
            }

            return totalRevenue;
        }

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
                    totalRevenue += appointment.Doctor.ConsultationFees;
            }

            return totalRevenue;
        }

        public async Task<decimal> GetTotalRevenueInSpecificMonthAsync(int year, int month)
        {
            if (year < 1) throw new ArgumentException("Invalid year");
            if (month < 1 || month > 12) throw new ArgumentException("Invalid month");

            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a =>
                    a.Status == AppointmentStatus.Completed &&
                    a.Date.Year == year &&
                    a.Date.Month == month
                );

            decimal totalRevenue = 0;
            foreach (var appointment in appointments)
            {
                if (appointment.Doctor != null)
                    totalRevenue += appointment.Doctor.ConsultationFees;
            }

            return totalRevenue;
        }

        public async Task<decimal> GetTotalRevenueInSpecificYearAsync(int year)
        {
            if (year < 1) throw new ArgumentException("Invalid year");

            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a =>
                    a.Status == AppointmentStatus.Completed &&
                    a.Date.Year == year
                );

            decimal totalRevenue = 0;
            foreach (var appointment in appointments)
            {
                if (appointment.Doctor != null)
                    totalRevenue += appointment.Doctor.ConsultationFees;
            }

            return totalRevenue;
        }

        // للـ Line Chart - Monthly Revenue Trend
        public async Task<List<object>> GetMonthlyRevenue(int year)
        {
            if (year < 1) throw new ArgumentException("Invalid year");

            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a => a.Status == AppointmentStatus.Completed && a.Date.Year == year)
                .ToList();

            var monthlyRevenue = new List<object>();
            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int month = 1; month <= 12; month++)
            {
                var monthAppointments = appointments.Where(a => a.Date.Month == month);

                decimal revenue = 0;
                foreach (var appointment in monthAppointments)
                {
                    if (appointment.Doctor != null)
                        revenue += appointment.Doctor.ConsultationFees;
                }

                monthlyRevenue.Add(new
                {
                    monthName = monthNames[month - 1],
                    month = month,
                    revenue = revenue
                });
            }

            return monthlyRevenue;
        }

        // للـ Bar Chart - Revenue by Branch
        public async Task<List<object>> GetRevenueByBranch()
        {
            var branches = await _branchRepository.GetAllAsync();
            var appointments = (await _appointmentRepository.GetAllAsync())
                .Where(a => a.Status == AppointmentStatus.Completed)
                .ToList();

            var branchRevenue = new List<object>();

            foreach (var branch in branches)
            {
                var branchAppointments = appointments.Where(a => a.BranchId == branch.BranchId);

                decimal revenue = 0;
                foreach (var appointment in branchAppointments)
                {
                    if (appointment.Doctor != null)
                        revenue += appointment.Doctor.ConsultationFees;
                }

                branchRevenue.Add(new
                {
                    branchId = branch.BranchId,
                    branchName = branch.BranchName,
                    revenue = revenue
                });
            }

            return branchRevenue;
        }
    }
}
