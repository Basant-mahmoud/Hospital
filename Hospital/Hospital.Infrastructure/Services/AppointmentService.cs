using AutoMapper;
using Clinic.Infrastructure.Persistence;
using Hospital.Application.DTO.Appointment;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Payment;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IBranchRepository _branchRepo;
        private readonly IPaymentRepository _PaymentRepo;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepo,
            IPatientRepository patientRepo,
            IDoctorRepository doctorRepository,
            IBranchRepository branchRepo,
            IMapper mapper,
            IPaymentRepository PaymentRepo,
            AppDbContext dbContext,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepository = doctorRepository;
            _branchRepo = branchRepo;
            _mapper = mapper;
            _PaymentRepo = PaymentRepo;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AppointmentDto> AddAsync(AddAppointmentDto dto)
        {
            _logger.LogInformation(
                 "Creating appointment for PatientId: {PatientId}, DoctorId: {DoctorId}, Date: {Date}, Shift: {Shift}",
                 dto.PatientId, dto.DoctorId, dto.Date, dto.Shift);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", dto.PatientId);
                    throw new ArgumentException("Patient does not exist.");
                }

                var doctor = await _doctorRepository.GetAsync(dto.DoctorId);
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor with ID {DoctorId} not found", dto.DoctorId);
                    throw new ArgumentException("Doctor does not exist.");
                }

                var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
                if (branch == null)
                {
                    _logger.LogWarning("Branch with ID {BranchId} not found", dto.BranchId);
                    throw new ArgumentException("Branch does not exist.");
                }

                if (dto.PaymentMethod != PaymentMethod.Cash && dto.PaymentMethod != PaymentMethod.Paymob)
                {
                    _logger.LogWarning("Invalid payment method: {PaymentMethod}", dto.PaymentMethod);
                    throw new ArgumentException("Invalid payment method. Allowed: Cash or Paymob");
                }

                if (dto.Date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                    throw new ArgumentException("Appointment date cannot be in the past.");

                // Get shift start/end as TimeSpan
                var (shiftStartSpan, shiftEndSpan) = ShiftTimeRanges.GetShiftRange(dto.Shift);

                // Convert shift TimeSpan → DateTime for comparison
                var shiftStartUtc = dto.Date.ToDateTime(TimeOnly.FromTimeSpan(shiftStartSpan));
                var shiftEndUtc = dto.Date.ToDateTime(TimeOnly.FromTimeSpan(shiftEndSpan));

                var nowUtc = DateTime.UtcNow;

                // Check if shift is in the past (even if date is today)
                if (nowUtc > shiftEndUtc)
                {
                    throw new ArgumentException(
                        $"Cannot book appointment: the selected shift ({shiftStartUtc:HH:mm} - {shiftEndUtc:HH:mm}) has already passed.");
                }

                // Validate: patient already booked same doctor same shift
                if (await _appointmentRepo.PatientBookedSameDoctorSameShiftAsync(dto.PatientId, dto.DoctorId, dto.Date, dto.Shift))
                {
                    throw new ArgumentException("Patient already has an appointment with this doctor in this shift.");
                }

                var appointment = _mapper.Map<Appointment>(dto);
                appointment.CreatedAt = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;
                appointment.Status = AppointmentStatus.Confirmed;

                var created = await _appointmentRepo.AddAsync(appointment);

                if (created != null && created.PaymentMethod == PaymentMethod.Cash)
                {
                    var payment = await _PaymentRepo.CreatePendingPaymentAsync(
                        created.AppointmentId,
                        doctor.ConsultationFees,
                        "EGP"
                    );

                    if (payment == null)
                    {
                        _logger.LogError("Failed to create payment for AppointmentId: {AppointmentId}", created.AppointmentId);
                        throw new InvalidOperationException("Cannot create payment");
                    }
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Appointment created successfully with ID: {AppointmentId}", created.AppointmentId);

                return _mapper.Map<AppointmentDto>(created);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create appointment for PatientId: {PatientId}, DoctorId: {DoctorId}", dto.PatientId, dto.DoctorId);
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting appointment with ID: {AppointmentId}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid appointment ID: {AppointmentId}", id);
                throw new ArgumentException("Invalid appointment ID.");
            }

            var record = await _appointmentRepo.GetAsync(id);
            if (record == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found", id);
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");
            }

            var deleted = await _appointmentRepo.DeleteAsync(record);
            _logger.LogInformation("Appointment with ID {AppointmentId} deleted successfully", id);

            return deleted;
        }

        public async Task<List<AppointmentDto>> GetByDoctorId(int DoctorId)
        {
            _logger.LogInformation("Fetching appointments for DoctorId: {DoctorId}", DoctorId);

            if (DoctorId <= 0)
                throw new ArgumentException("Invalid doctor ID.");

            var doctor = await _doctorRepository.GetAsync(DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", DoctorId);
                throw new KeyNotFoundException($"Doctor with ID {DoctorId} does not exist.");
            }

            var records = await _appointmentRepo.GetByDoctorIdAsync(DoctorId);
            return _mapper.Map<List<AppointmentDto>>(records ?? new List<Appointment>());
        }

        public async Task<List<AppointmentDto>> GetAllAppointmentCancelByDoctorId(int DoctorId)
        {
            _logger.LogInformation("Fetching appointments for DoctorId: {DoctorId}", DoctorId);

            if (DoctorId <= 0)
                throw new ArgumentException("Invalid doctor ID.");

            var doctor = await _doctorRepository.GetAsync(DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", DoctorId);
                throw new KeyNotFoundException($"Doctor with ID {DoctorId} does not exist.");
            }

            var records = await _appointmentRepo.GetCancelByDoctorIdAsync(DoctorId);
            return _mapper.Map<List<AppointmentDto>>(records ?? new List<Appointment>());
        }

        public async Task<AppointmentDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching appointment by ID: {AppointmentId}", id);

            if (id <= 0)
                throw new ArgumentException("Invalid appointment ID.");

            var record = await _appointmentRepo.GetAsync(id);
            if (record == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found", id);
                throw new KeyNotFoundException($"Appointment with ID {id} does not exist.");
            }

            return _mapper.Map<AppointmentDto>(record);
        }

        public async Task<List<AppointmentDto>> GetByPatientId(int PatientId)
        {
            _logger.LogInformation("Fetching appointments for PatientId: {PatientId}", PatientId);

            if (PatientId <= 0)
                throw new ArgumentException("Invalid patient ID.");

            var patient = await _patientRepo.GetByIdAsync(PatientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", PatientId);
                throw new KeyNotFoundException($"Patient with ID {PatientId} does not exist.");
            }

            var records = await _appointmentRepo.GetByPatientIdAsync(PatientId);
            return _mapper.Map<List<AppointmentDto>>(records ?? new List<Appointment>());
        }

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all appointments");
            var records = await _appointmentRepo.GetAllAsync();
            return _mapper.Map<List<AppointmentDto>>(records ?? new List<Appointment>());
        }

        public async Task<string> MarkAsCompletedAsync(int id)
        {
            var appointment = await _appointmentRepo.GetAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found", id);
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");
            }
            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.UtcNow;
            var result = await _appointmentRepo.UpdateAsync(appointment);
            _logger.LogInformation("Appointment with ID {AppointmentId} marked as completed", id);
            if (result == null)
            {
                _logger.LogError("Failed to update appointment with ID {AppointmentId}", id);
                throw new InvalidOperationException("Failed to update appointment status.");

            }
            return "Appointment marked as completed successfully.";
        }
    }
}
