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

        [HttpGet("throw")]
        public IActionResult ThrowError()
        {
            throw new Exception("This is a test exception!");
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            // Validate email format
            if (!ValidationHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format");
                return BadRequest(ModelState);
            }

            // Validate model state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call AuthService
            var result = await _authService.RegisterAsync(model);

            // If registration failed, return bad request
            if (!result.IsRegistered)
            {
                ModelState.AddModelError("RegistrationError", result.Message);
                return BadRequest(ModelState);
            }

            return Ok(result);
        }


        [HttpPost("register-from-excel")]
        public async Task<IActionResult> RegisterFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var users = new List<RegisterModel>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        return BadRequest("No worksheet found in Excel file.");

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var model = new RegisterModel
                        {
                            Username = worksheet.Cells[row, 1].Text,
                            Email = worksheet.Cells[row, 2].Text,
                            Name = worksheet.Cells[row, 3].Text,
                            Role = worksheet.Cells[row, 5].Text,
                            PhoneNumber = worksheet.Cells[row, 4].Text,
                            Password = worksheet.Cells[row, 6].Text
                        };

                        users.Add(model);
                    }
                }
            }

            var results = new List<RegisterDto>();

            foreach (var user in users)
            {
                var result = await _authService.RegisterAsync(user);
                results.Add(result);
            }

            return Ok(results);
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
        [Authorize] // This endpoint should be authorized
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
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            if (!result) return BadRequest("Could not process request.");
            return Ok("If your email is registered and confirmed, you will receive a reset link.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
            if (!result) return BadRequest("Reset password failed.");
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


    }

}
