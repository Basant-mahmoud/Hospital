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

            //if (!authResult.IsAuthenticated)
            //    throw new ValidationException("Failed to create user: " + authResult.Message);

            // 2) Map dto -> Doctor
            var doctor = _mapper.Map<Doctor>(dto);

            // 3) Extract UserId from token
            //var userId = TokenHelper.GetUserIdFromToken(authResult.Token);
           // doctor.UserId = userId;

            doctor.CreatedAt = DateTime.UtcNow;
            doctor.UpdatedAt = DateTime.UtcNow;

            // 4) Add branches (Many-to-Many)
            doctor.Branches = new List<Branch>();
            foreach (var branchId in dto.BranchIds.Distinct())
            {
                var branch = await _branchRepo.GetByIdAsync(branchId);
                if (branch != null)
                    doctor.Branches.Add(branch);
            }

            // 5) Save doctor
            var created = await _doctorRepo.AddAsync(doctor);

            // 6) Send email to doctor with credentials
            var emailBody = $@"
                <p> hi doctor {dto.Name},</p>
                <p>You are registed to our system you can check your account by use  email and password</p>
                <ul>
                    <li>Username: {dto.Username}</li>
                    <li>Email: {dto.Email}</li>
                    <li>Password: {dto.Password}</li>
                </ul>
                <p>Best grade from our team</p>";

            try
            {
                await _emailService.SendEmailAsync(dto.Email, "register doctor account done", emailBody);
            }
            catch(Exception ex)
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
            }

            // Update Branch based on BranchID
            if (dto.BranchID != null && dto.BranchID.Any())
            {
                doctor.Branches = new List<Branch>();
                foreach (var branchId in dto.BranchID.Distinct())
                {
                    var branch = await _branchRepo.GetByIdAsync(branchId);
                    if (branch != null)
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
