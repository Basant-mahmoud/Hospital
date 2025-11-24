using AutoMapper;
using Clinic.Infrastructure.Persistence;
using Hospital.Application.DTO.Appointment;
using Hospital.Application.DTO.Doctor;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Payment;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Hospital.Infrastructure.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IBranchRepository _branchRepo;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;
        private readonly AppDbContext _context;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentRepository _paymentRepository;

        public DoctorService(IMapper mapper, IDoctorRepository doctorRepo, IBranchRepository branchRepo, IEmailService emailService,
            IAuthService authService, ILogger<DoctorService> logger, IAppointmentRepository appointmentRepository, IPaymentRepository paymentRepository,
            AppDbContext context)
        {
            _mapper = mapper;
            _doctorRepo = doctorRepo;
            _branchRepo = branchRepo;
            _emailService = emailService;
            _authService = authService;
            _logger = logger;
            _context = context;
            _appointmentRepository = appointmentRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<DoctorDto> AddAsync(AddDoctorDto dto)
        {
            _logger.LogInformation("Adding new doctor: {DoctorName}, Email: {Email}", dto.Name, dto.Email);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var specialization = await _context.Specializations.FindAsync(dto.SpecializationId);
                if (specialization == null)
                {
                    _logger.LogWarning("Specialization with ID {SpecializationId} not found", dto.SpecializationId);
                    throw new KeyNotFoundException($"Specialization with ID {dto.SpecializationId} does not exist.");
                }

                var registerModel = new RegisterModel
                {
                    Email = dto.Email,
                    Username = dto.Username,
                    Password = dto.Password,
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    Role = "Doctor"
                };

                var authResult = await _authService.RegisterAsync(registerModel);
                if (!authResult.IsRegistered || string.IsNullOrEmpty(authResult.UserId))
                {
                    _logger.LogError("Failed to register user for doctor {DoctorName}: {Message}", dto.Name, authResult.Message);
                    throw new InvalidOperationException("Failed to create user: " + authResult.Message);
                }

                var doctor = _mapper.Map<Doctor>(dto);
                doctor.UserId = authResult.UserId;
                doctor.CreatedAt = DateTime.UtcNow;
                doctor.UpdatedAt = DateTime.UtcNow;

                var distinctBranchIds = dto.BranchIds.Distinct().ToList();
                var branches = await _branchRepo.GetByIdsAsync(distinctBranchIds);

                var missingBranchIds = distinctBranchIds.Except(branches.Select(b => b.BranchId)).ToList();
                if (missingBranchIds.Any())
                {
                    _logger.LogWarning("Branches not found: {BranchIds}", string.Join(", ", missingBranchIds));
                    throw new ArgumentException($"Branches not found: {string.Join(", ", missingBranchIds)}");
                }

                doctor.Branches = branches.ToList();

                var createdDoctor = await _doctorRepo.AddAsync(doctor);
                await transaction.CommitAsync();

                try
                {
                    var emailBody = $@"
                        <p>Hi Dr. {dto.Name},</p>
                        <p>Your account has been created. You can log in with the following credentials:</p>
                        <ul>
                            <li>Username: {dto.Username}</li>
                            <li>Email: {dto.Email}</li>
                            <li>Password: {dto.Password}</li>
                        </ul>
                        <p>Best regards,</p>
                        <p>The Team</p>";

                    await _emailService.SendEmailAsync(dto.Email, "Doctor Account Registration", emailBody);
                    _logger.LogInformation("Sent registration email to {Email}", dto.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to send email to doctor {Email}: {Message}", dto.Email, ex.Message);
                }

                return _mapper.Map<DoctorDto>(createdDoctor);
            }
            catch
            {
                await transaction.RollbackAsync();
                _logger.LogError("Transaction rolled back while adding doctor {DoctorName}", dto.Name);
                throw;
            }
        }

        public async Task<int> UpdateAsync(UpdateDoctorDto dto)
        {
            _logger.LogInformation("Updating doctor with ID {DoctorId}", dto.DoctorId);

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var doctor = await _doctorRepo.GetAsync(dto.DoctorId)
                ?? throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found.");

            _mapper.Map(dto, doctor);

            if (doctor.User != null)
            {
                doctor.User.UserName = dto.UserName;
                doctor.User.Email = dto.Email;
                doctor.User.FullName = dto.FullName;
                doctor.User.NormalizedUserName = dto.UserName?.ToUpper();
                doctor.User.NormalizedEmail = dto.Email?.ToUpper();
            }

            if (dto.BranchID != null && dto.BranchID.Any())
            {
                doctor.Branches = new List<Branch>();
                foreach (var branchId in dto.BranchID.Distinct())
                {
                    var branch = await _branchRepo.GetByIdAsync(branchId);
                    if (branch == null)
                    {
                        _logger.LogWarning("Branch with ID {BranchId} not found during update", branchId);
                        throw new ArgumentException($"Cannot update doctor. Branch with ID {branchId} does not exist.");
                    }

                    doctor.Branches.Add(branch);
                }
            }

            doctor.UpdatedAt = DateTime.UtcNow;
            var result = await _doctorRepo.UpdateAsync(doctor);
            _logger.LogInformation("Doctor with ID {DoctorId} updated successfully", dto.DoctorId);

            return result;
        }

        public async Task UpdatePersonalInfoAsync(DoctorSelfUpdateDto dto)
        {
            _logger.LogInformation("Updating personal info for doctor ID {DoctorId}", dto.DoctorId);

            if (dto == null)
            {
                _logger.LogError("DoctorSelfUpdateDto is null");
                throw new ArgumentNullException(nameof(dto));
            }

            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", dto.DoctorId);
                throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found.");
            }

            _mapper.Map(dto, doctor);

            if (doctor.User != null)
            {
                doctor.User.FullName = dto.FullName ?? doctor.User.FullName;
                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    doctor.User.PhoneNumber = dto.PhoneNumber;
            }

            doctor.UpdatedAt = DateTime.UtcNow;
            await _doctorRepo.UpdateAsync(doctor);

            _logger.LogInformation("Personal info updated successfully for doctor ID {DoctorId}", dto.DoctorId);
        }

        public async Task<int> DeleteAsync(GetDoctorDto dto)
        {
            _logger.LogInformation("Deleting doctor with ID {DoctorId}", dto.DoctorId);

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existingDoctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (existingDoctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found for deletion", dto.DoctorId);
                throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found.");
            }

            var result = await _doctorRepo.DeleteAsync(dto.DoctorId);
            _logger.LogInformation("Doctor with ID {DoctorId} deleted successfully", dto.DoctorId);

            return result;
        }

        public async Task<DoctorDto?> GetAsync(GetDoctorDto dto)
        {
            _logger.LogInformation("Fetching doctor with ID {DoctorId}", dto.DoctorId);

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", dto.DoctorId);
                return null;
            }

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync(int branchId)
        {
            _logger.LogInformation("Fetching all doctors for branch ID {BranchId}", branchId);
            if (branchId <= 0)
            {
                _logger.LogWarning("Invalid BranchId: {BranchId}", branchId);
                throw new ArgumentException("BranchId must be greater than zero.");
            }
            var branch = await _branchRepo.GetByIdAsync(branchId);
            if (branch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", branchId);
                throw new KeyNotFoundException("Branch not found.");
            }

            var doctors = await _doctorRepo.GetAllByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorInSystemAsync()
        {
            _logger.LogInformation("Fetching all doctors in the system");
            var doctors = await _doctorRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationIdAsync(int specializationId)
        {
            _logger.LogInformation("Fetching doctors for specialization ID {SpecializationId}", specializationId);

            if (specializationId <= 0)
            {
                _logger.LogWarning("Invalid SpecializationId: {SpecializationId}", specializationId);
                throw new ArgumentException("SpecializationId must be greater than zero.");
            }
            var specialization = await _context.Specializations.FindAsync(specializationId);
            if (specialization == null)
            {
                 _logger.LogWarning("Specialization with ID {SpecializationId} not found", specializationId);
                throw new KeyNotFoundException($"Specialization with ID {specializationId} does not exist.");
            }
            var doctors = await _doctorRepo.GetDoctorsBySpecializationIdAsync(specializationId);
            if (doctors == null || !doctors.Any())
            {
                _logger.LogWarning("No doctors found for specialization ID {SpecializationId}", specializationId);
                throw new KeyNotFoundException($"No doctors found for specialization ID {specializationId}.");
            }

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<List<AppoinmentandPaientDetaliesDto>> GetTodayForDoctorAsync(int doctorId)
        {
            _logger.LogInformation("Fetching today's appointments for doctor ID {DoctorId}", doctorId);

            if (doctorId <= 0)
            {
                _logger.LogWarning("Invalid DoctorId: {DoctorId}", doctorId);
                throw new ArgumentException("Invalid doctor ID.");
            }

            var doctor = await _doctorRepo.GetAsync(doctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} does not exist", doctorId);
                throw new KeyNotFoundException($"Doctor with ID {doctorId} does not exist.");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var appointments = await _doctorRepo.GetAppoimentsByDateForDoctorAsync(doctorId, today);

            if (appointments == null || !appointments.Any())
            {
                _logger.LogInformation("No appointments found for doctor ID {DoctorId} today", doctorId);
                return new List<AppoinmentandPaientDetaliesDto>();
            }

            return _mapper.Map<List<AppoinmentandPaientDetaliesDto>>(appointments);
        }
        public async Task<List<AppoinmentandPaientDetaliesDto>> GetAppoinmentsForDoctorByDateAsync(int doctorId , DateOnly date)
        {
            _logger.LogInformation("Fetching {date}'s appointments for doctor ID {DoctorId}",date , doctorId);

            if (doctorId <= 0) throw new ArgumentException("Invalid doctor ID.");

            var doctor = await _doctorRepo.GetAsync(doctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} does not exist", doctorId);
                throw new KeyNotFoundException($"Doctor with ID {doctorId} does not exist.");
            }

            if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                _logger.LogWarning("The provided date {Date} is in the past", date);
                throw new ArgumentException("The provided date cannot be in the past.");
            }

            var appointments = await _doctorRepo.GetAppoimentsByDateForDoctorAsync(doctorId, date);

            if (appointments == null || !appointments.Any())
            {
                _logger.LogInformation("No appointments found for doctor ID {DoctorId} today", doctorId);
                return new List<AppoinmentandPaientDetaliesDto>();
            }

            return _mapper.Map<List<AppoinmentandPaientDetaliesDto>>(appointments);
        }

        public async Task<bool> convertStatuesOFPaymentToPayied(int appointmentId)
        {
            _logger.LogInformation("Converting payment status to Paid for appointment ID {AppointmentId}", appointmentId);

            var payment = await _paymentRepository.GetPaymentByAppointmentIdAsync(appointmentId);
            if (payment == null)
            {
                _logger.LogWarning("No payment found for appointment ID {AppointmentId}", appointmentId);
                return false;
            }

            payment.Status = PaymentStatus.Paid;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdatePaymentAsync(payment);
            _logger.LogInformation("Payment status updated to Paid for appointment ID {AppointmentId}", appointmentId);

            return true;
        }

        public async Task<int> CancelAppointmentsForDoctorbyDate(int id, DateOnly date)
        {
            _logger.LogInformation("Cancelling appointments for doctor ID {DoctorId} on date {Date}", id, DateTime.UtcNow);

            var doctor = await _doctorRepo.GetAsync(id);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} does not exist", id);
                throw new KeyNotFoundException($"Doctor with ID {id} does not exist.");
            }

            var appointments = await _doctorRepo.GetAppoimentsByDateForDoctorAsync(id, date);
            if (appointments == null || !appointments.Any())
            {
                _logger.LogInformation("No appointments found for doctor ID {DoctorId} on date {Date}", id, DateTime.UtcNow);
                return 0;
            }

            // حددنا اللي محتاج يتلغى
            var toCancel = appointments.Where(a => a.Status != AppointmentStatus.Cancelled).ToList();
            foreach (var appointment in toCancel)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.UpdatedAt = DateTime.UtcNow;

                // إرسال الإيميلات
                var html = $@"
        <p>Dear {appointment.Patient.User.FullName},</p>
        <p>We regret to inform you that your appointment scheduled for <b>{appointment.Date}</b> with <b>Dr. {appointment.Doctor.User.FullName}</b> has been cancelled due to unforeseen circumstances.</p>
        <p>We sincerely apologize for any inconvenience this may cause. Our team will be happy to assist you in booking another available time that best fits your schedule.</p>
        <p>If you would like to reschedule or need additional support, please feel free to contact us at your convenience.</p>
        <p>Kind regards,<br/>Hospital Support Team</p>";

                try
                {
                    await _emailService.SendEmailAsync(appointment.Patient.User.Email, "Appointment Cancellation", html);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send cancellation email for appointment {AppointmentId}", appointment.AppointmentId);
                }
            }

            // تحديث كل العناصر مرة واحدة
            _appointmentRepository.UpdateRange(toCancel);

            _logger.LogInformation("Cancelled {CancelledCount} appointments for doctor ID {DoctorId} on date {Date}", toCancel.Count, id, DateTime.UtcNow);
            return toCancel.Count;
        }


    }
}
