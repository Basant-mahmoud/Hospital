using AutoMapper;
using Clinic.Infrastructure.Persistence;
using Hospital.Application.DTO.Appointment;
using Hospital.Application.DTO.Doctor;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

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
        private readonly AppDbContext _context ;


        public DoctorService(
        IMapper mapper,
        IDoctorRepository doctorRepo,
        IBranchRepository branchRepo,
        IEmailService emailService,
        IAuthService authService,
        ILogger<DoctorService> logger,
        AppDbContext context)
        {
            _mapper = mapper;
            _doctorRepo = doctorRepo;
            _branchRepo = branchRepo;
            _emailService = emailService;
            _authService = authService;
            _logger = logger;
            _context = context;
        }

      
        public async Task<DoctorDto> AddAsync(AddDoctorDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 0️⃣ Validate Specialization
                var specialization = await _context.Specializations.FindAsync(dto.SpecializationId);
                if (specialization == null)
                    throw new KeyNotFoundException($"Specialization with ID {dto.SpecializationId} does not exist.");

                // 1️⃣ Register user via AuthService
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
                    throw new InvalidOperationException("Failed to create user: " + authResult.Message);

                // 2️⃣ Map DTO -> Doctor entity
                var doctor = _mapper.Map<Doctor>(dto);
                doctor.UserId = authResult.UserId;
                doctor.CreatedAt = DateTime.UtcNow;
                doctor.UpdatedAt = DateTime.UtcNow;

                // 3️⃣ Validate and assign branches
                var distinctBranchIds = dto.BranchIds.Distinct().ToList();
                var branches = await _branchRepo.GetByIdsAsync(distinctBranchIds);

                var missingBranchIds = distinctBranchIds.Except(branches.Select(b => b.BranchId)).ToList();
                if (missingBranchIds.Any())
                    throw new ArgumentException($"Branches not found: {string.Join(", ", missingBranchIds)}");

                doctor.Branches = branches.ToList();

                // 4️⃣ Save doctor
                var createdDoctor = await _doctorRepo.AddAsync(doctor);

                // 5️⃣ Commit transaction
                await transaction.CommitAsync();

                // 6️⃣ Send email to doctor with credentials
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
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to send email to doctor {DoctorEmail}: {Message}", dto.Email, ex.Message);
                }

                // 7️⃣ Return DTO
                return _mapper.Map<DoctorDto>(createdDoctor);
            }
            catch
            {
                // Rollback transaction if any exception occurs
                await transaction.RollbackAsync();
                throw;
            }
        }





        public async Task<int> UpdateAsync(UpdateDoctorDto dto)
        {
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

            // Update Branch based on BranchID
            if (dto.BranchID != null && dto.BranchID.Any())
            {
                doctor.Branches = new List<Branch>();
                foreach (var branchId in dto.BranchID.Distinct())
                {
                    var branch = await _branchRepo.GetByIdAsync(branchId);
                    if (branch == null)
                        throw new ArgumentException($"Cannot update doctor. Branch with ID {branchId} does not exist.");

                    doctor.Branches.Add(branch);
                }
            }

            doctor.UpdatedAt = DateTime.UtcNow;

            return await _doctorRepo.UpdateAsync(doctor);
        }

        public async Task UpdatePersonalInfoAsync(DoctorSelfUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // 0️⃣ Validate Specialization
            var specialization = await _context.Specializations.FindAsync(dto.SpecializationId);
            if (specialization == null)
                throw new KeyNotFoundException($"Specialization with ID {dto.SpecializationId} does not exist.");

            // 1️⃣ Get existing doctor
            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found.");

            // 2️⃣ Map DTO to entity (ignores User & Branches)
            _mapper.Map(dto, doctor);

            // 3️⃣ Update User info manually
            if (doctor.User != null)
            {
                doctor.User.FullName = dto.FullName ?? doctor.User.FullName;
                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    doctor.User.PhoneNumber = dto.PhoneNumber;
            }

            // 4️⃣ Update branches if any
            if (dto.BranchIds != null && dto.BranchIds.Any())
            {
                var branches = await _branchRepo.GetByIdsAsync(dto.BranchIds.Distinct());
                doctor.Branches = branches.ToList();
            }

            // 5️⃣ Update timestamps
            doctor.UpdatedAt = DateTime.UtcNow;

            // 6️⃣ Save changes
            await _doctorRepo.UpdateAsync(doctor);
        }



        public async Task<int> DeleteAsync(GetDoctorDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var existingDoctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (existingDoctor == null) throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found.");

            return await _doctorRepo.DeleteAsync(dto.DoctorId);
        }

        public async Task<DoctorDto?> GetAsync(GetDoctorDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var doctor = await _doctorRepo.GetAsync(dto.DoctorId);
            if (doctor == null) return null;

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync(int branchId)
        {
            if (branchId <= 0) throw new ArgumentException("BranchId must be greater than zero.");
            var doctors = await _doctorRepo.GetAllByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorInSystemAsync()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }



        public async Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationIdAsync(int specializationId)
        {
            if (specializationId <= 0)
                throw new ArgumentException("SpecializationId must be greater than zero.");

            var doctors = await _doctorRepo.GetDoctorsBySpecializationIdAsync(specializationId);

            if (doctors == null || !doctors.Any())
                throw new KeyNotFoundException($"No doctors found for specialization ID {specializationId}.");

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        public async Task<List<AppoinmentandPaientDetaliesDto>> GetTodayForDoctorAsync(int doctorId)
        {
            if (doctorId <= 0)
                throw new ArgumentException("Invalid doctor ID.");

            var doctor = await _doctorRepo.GetAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {doctorId} does not exist.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var appointments = await _doctorRepo.GetTodayCompletedForDoctorAsync(doctorId, today);

            if (appointments == null || !appointments.Any())
                return new List<AppoinmentandPaientDetaliesDto>();

            return _mapper.Map<List<AppoinmentandPaientDetaliesDto>>(appointments);
        }



    }
}
