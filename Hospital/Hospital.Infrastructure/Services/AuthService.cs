using Clinic.Infrastructure.Persistence;
using Hospital.Application.DTO.Auth;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Hospital.Application.DTO.Admin;

namespace Hospital.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly JWT _jwt;
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IPatientRepository _patientRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly AppDbContext _dbContext;

        public AuthService(
            IOptions<JWT> jwt,
            IAuthRepository authRepository,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IPatientRepository patientRepository,
            AppDbContext context,
            ILogger<AuthService> logger,
            AppDbContext appDbContext)
        {
            _jwt = jwt.Value;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _patientRepository = patientRepository;
            _context = context;
            _logger = logger;
            _dbContext = appDbContext;
        }

        public async Task<RegisterDto> RegisterAsync(RegisterModel model)
        {
            _logger.LogInformation("Registration attempt for email: {Email}, role: {Role}", model.Email, model.Role);

            if (await _authRepository.EmailExistAsync(model.Email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", model.Email);
                return new RegisterDto { Message = "Email already exists!" };
            }

            if (await _authRepository.UsernameExistsAsync(model.Username))
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", model.Username);
                return new RegisterDto { Message = "Username already exists!" };
            }

            if (string.IsNullOrEmpty(model.Role) || (model.Role != "Patient" && model.Role != "Doctor" && model.Role != "Admin"))
            {
                _logger.LogWarning("Registration failed: Invalid role {Role} for user {Email}", model.Role, model.Email);
                return new RegisterDto { Message = "Invalid role. Only 'Patient', 'Doctor' and 'Admin' allowed." };
            }

            // Ensure role exists before creating user
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                _logger.LogInformation("Creating new role: {Role}", model.Role);
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.Name,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed for {Email}: {Errors}",
                    model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return new RegisterDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
            }

            await _userManager.AddToRoleAsync(user, model.Role);
            _logger.LogInformation("User {Email} assigned to role {Role}", model.Email, model.Role);

            if (model.Role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    User = user,
                    EmergencyContact = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _patientRepository.AddAsync(patient);
                _logger.LogInformation("Patient record created for user {UserId}", user.Id);
            }

            _logger.LogInformation("Registration successful for user {Email} with ID {UserId}", model.Email, user.Id);
            return new RegisterDto
            {
                Message = "Account registered successfully. Please login.",
                IsRegistered = true,
                UserId = user.Id
            };
        }

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            _logger.LogInformation("Login attempt for email: {Email}", model.Email);

            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.LogWarning("Login failed for email: {Email} - Invalid credentials", model.Email);
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            authModel.RefreshToken = refreshToken;
            authModel.RefreshTokenExpiration = user.RefreshTokenExpiryTime;

            if (rolesList.Contains("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor != null)
                {
                    authModel.DoctorId = doctor.DoctorId;
                    _logger.LogInformation("Doctor {DoctorId} logged in successfully", doctor.DoctorId);
                }
            }
            else if (rolesList.Contains("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient != null)
                {
                    authModel.PatientId = patient.PatientId;
                    _logger.LogInformation("Patient {PatientId} logged in successfully", patient.PatientId);
                }
            }

            _logger.LogInformation("User {Email} logged in successfully with roles: {Roles}",
                model.Email, string.Join(", ", rolesList));

            return authModel;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            _logger.LogInformation("Attempting to add role {Role} to user {UserId}", model.Role, model.UserId);

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null || !await _roleManager.RoleExistsAsync(model.Role))
            {
                _logger.LogWarning("Failed to add role: Invalid user ID {UserId} or role {Role}", model.UserId, model.Role);
                return "Invalid user ID or Role";
            }

            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                _logger.LogWarning("User {UserId} already has role {Role}", model.UserId, model.Role);
                return "User already assigned to this role";
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {Role} added to user {UserId} successfully", model.Role, model.UserId);
                return string.Empty;
            }
            else
            {
                _logger.LogError("Failed to add role {Role} to user {UserId}: {Errors}",
                    model.Role, model.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return "Something went wrong";
            }
        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            _logger.LogDebug("Creating JWT token for user {UserId}", user.Id);

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            _logger.LogDebug("JWT token created for user {UserId}, expires at {ExpiryTime}",
                user.Id, jwtSecurityToken.ValidTo);

            return jwtSecurityToken;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Password reset requested for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var code = GenerateRandomCode();
                _logger.LogInformation("Generated reset code for user {UserId}", user.Id);

                var resetCode = new PasswordResetCode
                {
                    UserId = user.Id,
                    Code = code,
                    ExpireAt = DateTime.UtcNow.AddMinutes(10)
                };

                _context.PasswordResetCodes.Add(resetCode);
                await _context.SaveChangesAsync();

                var html = $@"
                <p>Hi {user.FullName},</p>
                <p>Your password reset code is: <b>{code}</b></p>
                <p>This code is valid for 10 minutes.</p>";

                try
                {
                    await _emailService.SendEmailAsync(email, "Your verification code", html);
                    _logger.LogInformation("Password reset email sent successfully to {Email}", email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                    return false;
                }
            }
            else
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            }

            return true;
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            _logger.LogInformation("Verifying reset code for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Code verification failed: User not found for email {Email}", email);
                return false;
            }

            var resetCode = await _context.PasswordResetCodes
                .Where(c => c.UserId == user.Id && c.Code == code && c.ExpireAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (resetCode != null)
            {
                _logger.LogInformation("Reset code verified successfully for user {UserId}", user.Id);
                return true;
            }
            else
            {
                _logger.LogWarning("Invalid or expired reset code for user {UserId}", user.Id);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            _logger.LogInformation("Password reset attempt for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Password reset failed: User not found for email {Email}", email);
                return false;
            }

            var remove = await _userManager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
            {
                _logger.LogError("Failed to remove old password for user {UserId}: {Errors}",
                    user.Id, string.Join(", ", remove.Errors.Select(e => e.Description)));
                return false;
            }

            var add = await _userManager.AddPasswordAsync(user, newPassword);
            if (add.Succeeded)
            {
                _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
                return true;
            }
            else
            {
                _logger.LogError("Failed to set new password for user {UserId}: {Errors}",
                    user.Id, string.Join(", ", add.Errors.Select(e => e.Description)));
                return false;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            _logger.LogInformation("Refresh token requested");

            var authModel = new AuthModel();
            var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == token);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                _logger.LogWarning("Refresh token invalid or expired");
                authModel.Message = "Invalid or expired refresh token.";
                return authModel;
            }

            var jwtToken = await CreateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.name = user.FullName;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            authModel.ExpiresOn = jwtToken.ValidTo;
            authModel.RefreshToken = newRefreshToken;
            authModel.RefreshTokenExpiration = user.RefreshTokenExpiryTime;

            _logger.LogInformation("Refresh token generated successfully for user {UserId}", user.Id);

            return authModel;
        }

        public async Task<string?> GetUserIdByEmailAsync(string email)
        {
            _logger.LogDebug("Looking up user ID for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return null;
            }

            _logger.LogDebug("User ID found: {UserId} for email: {Email}", user.Id, email);
            return user.Id;
        }

        private string GenerateRandomCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<AdminDto> GetUserDetails(string userId)
        {
            _logger.LogInformation("Fetching user details for userId: {UserId}", userId);

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError("User not found with ID: {UserId}", userId);
                throw new Exception("User not found");
            }

            var adminDto = new AdminDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
            };

            _logger.LogInformation("User details retrieved successfully for userId: {UserId}", userId);
            return adminDto;
        }
    }
}
