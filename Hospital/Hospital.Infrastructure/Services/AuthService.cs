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
using System.Security.Claims;

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

        public AuthService(IOptions<JWT> jwt, IAuthRepository authRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IPatientRepository patientRepository, AppDbContext context)
        {
            _jwt = jwt.Value;
            _authRepository = authRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _patientRepository = patientRepository;
            _context = context;
        }

        public async Task<RegisterDto> RegisterAsync(RegisterModel model)
        {
            // Check if email exists
            if (await _authRepository.EmailExistAsync(model.Email))
                return new RegisterDto { Message = "Email already exists!" };

            // Check if username exists
            if (await _authRepository.UsernameExistsAsync(model.Username))
                return new RegisterDto { Message = "Username already exists!" };

            // Validate role
            if (string.IsNullOrEmpty(model.Role) || (model.Role != "Patient") && (model.Role != "Doctor")&& (model.Role != "Admin"))
                return new RegisterDto { Message = "Invalid role. Only 'Patient' ,'Doctor'  and 'Admin' allowed." };

            // Ensure role exists before creating user
            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            // Create user
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
                return new RegisterDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await _userManager.AddToRoleAsync(user, model.Role);

            // Create patient if role is Patient
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
            }


            return new RegisterDto
            {
                Message = "Account registered successfully. Please login.",
                IsRegistered = true,
                UserId = user.Id
            };
        }



        //public async Task<AuthModel> LoginAsync(LoginModel model)
        //{
        //    var authModel = new AuthModel();
        //    var user = await _userManager.FindByEmailAsync(model.Email);

        //    if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        authModel.Message = "Email or Password is incorrect!";
        //        return authModel;
        //    }

        //    var jwtSecurityToken = await CreateJwtToken(user);
        //    var rolesList = await _userManager.GetRolesAsync(user);

        //    // create refresh token
        //    var refreshToken = GenerateRefreshToken();
        //    user.RefreshToken = refreshToken;
        //    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); //for week
        //    await _userManager.UpdateAsync(user);

        //    authModel.IsAuthenticated = true;
        //    authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        //    authModel.Email = user.Email;
        //    authModel.Username = user.UserName;
        //    authModel.ExpiresOn = jwtSecurityToken.ValidTo;
        //    authModel.Roles = rolesList.ToList();
        //    authModel.RefreshToken = refreshToken;
        //    authModel.RefreshTokenExpiration = user.RefreshTokenExpiryTime;

        //    return authModel;
        //}


        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            // create refresh token
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); //for week
            await _userManager.UpdateAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            authModel.RefreshToken = refreshToken;
            authModel.RefreshTokenExpiration = user.RefreshTokenExpiryTime;

            // 🔹 إضافة DoctorId أو PatientId حسب الـ Role
            if (rolesList.Contains("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor != null)
                    authModel.DoctorId = doctor.DoctorId;
            }
            else if (rolesList.Contains("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient != null)
                    authModel.PatientId = patient.PatientId;
            }

            return authModel;
        }



        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                // Standard role claim so ASP.NET can evaluate [Authorize(Roles="...")]
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

                //// Optional: keep "roles" string claim if some clients expect it
                //roleClaims.Add(new Claim("roles", role));
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

            return jwtSecurityToken;
        }



        public async Task<bool> ForgotPasswordAsync(string email)
        {
            
            // seach abour email in database or not 
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // generate code to send it to email function that make rendom code of 6 digits
                var code = GenerateRandomCode();

                // Save code in database (valid for 10 minutes)
                var resetCode = new PasswordResetCode
                {
                    UserId = user.Id,
                    Code = code,
                    ExpireAt = DateTime.UtcNow.AddMinutes(10)
                };

               _context.PasswordResetCodes.Add(resetCode);
                await _context.SaveChangesAsync();

                // Email content
                var html = $@"
          <p>Hi {user.FullName},</p>
          <p>Your password reset code is: <b>{code}</b></p>
          <p>This code is valid for 10 minutes.</p>";


                

                try
                {
                    // send email
                    await _emailService.SendEmailAsync(email, "Your verification code", html);
                }
                catch (Exception ex)
                {
                    // any error in sending email
                    return false; 
                }
            }

            return true;
        }
        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            var resetCode = await _context.PasswordResetCodes
                .Where(c => c.UserId == user.Id && c.Code == code && c.ExpireAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();
            return resetCode != null;
        }

        // this  function to reset password
        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var remove = await _userManager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
                return false;

            var add = await _userManager.AddPasswordAsync(user, newPassword);
            return add.Succeeded;
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
            var authModel = new AuthModel();

            var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == token);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
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

            return authModel;
        }

        public async Task<string?> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            return user.Id;
        }
        private string GenerateRandomCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }






    }
}
