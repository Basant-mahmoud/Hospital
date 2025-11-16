using AutoMapper;
using Hospital.Application.DTO.Doctor;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
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


        public DoctorService(
        IMapper mapper,
        IDoctorRepository doctorRepo,
        IBranchRepository branchRepo,
        IEmailService emailService,
        IAuthService authService,
        ILogger<DoctorService> logger)
        {
            _mapper = mapper;
            _doctorRepo = doctorRepo;
            _branchRepo = branchRepo;
            _emailService = emailService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<DoctorDto> AddAsync(AddDoctorDto dto)
        {
            // 1) Register user via AuthService
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


            if (!authResult.IsRegistered)
                throw new InvalidOperationException("Failed to create user: " + authResult.Message);

            // 2) Map dto -> Doctor
            var doctor = _mapper.Map<Doctor>(dto);

            // 3)  find UserId
            var userId = await _authService.GetUserIdByEmailAsync(dto.Email);

            doctor.UserId = userId;

            doctor.CreatedAt = DateTime.UtcNow;
            doctor.UpdatedAt = DateTime.UtcNow;

            // 4) Validate Branches before adding doctor
            var distinctBranchIds = dto.BranchIds.Distinct().ToList();
            var branches = new List<Branch>();

            foreach (var branchId in distinctBranchIds)
            {
                var branch = await _branchRepo.GetByIdAsync(branchId);

                if (branch == null)
                {
                    // rollback user creation logic if needed
                    throw new ArgumentException($"Branch with ID {branchId} does not exist. Please create the branch first.");
                }

                branches.Add(branch);
            }

            doctor.Branches = branches;

            // 5) Save doctor
            var created = await _doctorRepo.AddAsync(doctor);

            // 6) Send email to doctor with credentials
            var emailBody = $@"
         <p>Hi doctor {dto.Name},</p>
         <p>You are registered to our system. You can check your account using the following credentials:</p>
         <ul>
             <li>Username: {dto.Username}</li>
             <li>Email: {dto.Email}</li>
             <li>Password: {dto.Password}</li>
         </ul>
         <p>Best regards from our team.</p>";

            try
            {
                await _emailService.SendEmailAsync(dto.Email, "Doctor Account Registration", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to send email to doctor {DoctorEmail}: {Message}", dto.Email, ex.Message);
            }

            return _mapper.Map<DoctorDto>(created);
        }



        public async Task<int> UpdateAsync(DoctorDto dto)
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

        public async Task<IEnumerable<DoctorDto>> GetAllEventInSystemAsync()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
    }
}
