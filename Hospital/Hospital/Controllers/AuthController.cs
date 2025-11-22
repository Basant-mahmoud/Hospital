using Hospital.Application.DTO.Auth;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.ComponentModel;
using OfficeOpenXml;
using System.Security.Claims;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;


    public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================
        // 🔹 TEST AUTHENTICATION
        // ============================
        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuthentication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("uid")
                         ?? "Not found";

            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            return Ok(new
            {
                Message = "You are authenticated!",
                UserId = userId,
                Roles = roles
            });
        }

        // ============================
        // 🔹 TEST ROLE AUTHORIZATION (Patient)
        // ============================
        [HttpGet("test-role")]
        [Authorize(Roles = "Patient")]
        public IActionResult TestRole()
        {
            return Ok(new
            {
                Message = "Authorization success! You have the Patient role."
            });
        }

        // ======================
        // Existing endpoints...
        // ======================

        [HttpGet("throw")]
        public IActionResult ThrowError()
        {
            throw new Exception("This is a test exception!");
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ValidationHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsRegistered)
            {
                ModelState.AddModelError("RegistrationError", result.Message);
                return BadRequest(ModelState);
            }

            return Ok(result);
        }





        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("addrole")]
        [Authorize]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || !ValidationHelper.IsValidEmail(dto.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format.");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            if (!result) return BadRequest("Could not process request.");
            return Ok("If your email is registered and confirmed, you will receive a reset link.");
        }

        [HttpPost("Verify-Code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || !ValidationHelper.IsValidEmail(dto.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format.");
            }
            if (string.IsNullOrWhiteSpace(dto.Code) || !System.Text.RegularExpressions.Regex.IsMatch(dto.Code, @"^\d{6}$"))
            {
                ModelState.AddModelError("Code", "Code must be exactly 6 digits.");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.VerifyCodeAsync(dto.Email, dto.Code);
            if (!result) return BadRequest("Invalid or expired code.");
            return Ok("Code verified successfully.");
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(dto.Email) || !ValidationHelper.IsValidEmail(dto.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format.");
            }

            // Validate password: not null and first letter capital
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || !char.IsUpper(dto.NewPassword[0]))
            {
                ModelState.AddModelError("NewPassword", "Password must start with an uppercase letter.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call service
            var result = await _authService.ResetPasswordAsync(dto.Email, dto.NewPassword);
            if (!result)
                return BadRequest("Reset password failed.");

            return Ok("Password has been reset successfully.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken tokendto)
        {
            var result = await _authService.RefreshTokenAsync(tokendto.Token);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        //////////////////test ////////////////////
        // Test endpoint
        [HttpGet("whoami")]
        [Authorize] // requires valid token
        public IActionResult WhoAmI()
        {
            var userId = User.FindFirst("uid")?.Value;
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                UserId = userId,
                Roles = roles
            });
        }

        // Test Admin-only
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok("You are Admin — Authorization works!");
        }

        // Test Doctor-only
        [HttpGet("doctor-only")]
        [Authorize(Roles = "Doctor")]
        public IActionResult DoctorOnly()
        {
            return Ok("You are Doctor — Authorization works!");
        }

        // Test Patient-only
        [HttpGet("patient-only")]
        [Authorize(Roles = "Patient")]
        public IActionResult PatientOnly()
        {
            return Ok("You are Patient — Authorization works!");
        }
    }

}


