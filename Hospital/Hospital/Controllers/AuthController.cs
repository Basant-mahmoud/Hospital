using Hospital.API.Controllers;
using Hospital.Application.DTO.Auth;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            _logger.LogInformation("Register called at {time}", DateTime.Now);

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
            _logger.LogInformation("Login called at {time}", DateTime.Now);

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
            _logger.LogInformation("AddRole called at {time}", DateTime.Now);

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
            _logger.LogInformation("ForgotPassword called at {time}", DateTime.Now);

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
            _logger.LogInformation("VerifyCode called at {time}", DateTime.Now);


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
            _logger.LogInformation("ResetPassword called at {time}", DateTime.Now);

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
            _logger.LogInformation("RefreshToken called at {time}", DateTime.Now);

            var result = await _authService.RefreshTokenAsync(tokendto.Token);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("getAdminData")]
        [Authorize(Roles ="Admin")]  // Make sure the endpoint requires a token
        public IActionResult GetAdminData()
        {
            _logger.LogInformation("GetAdminData called at {time}", DateTime.Now);

            // Get User ID from JWT claim "uid"
            var userId = User.FindFirst("uid")?.Value;

            if (userId == null)
                return Unauthorized("Invalid token or user ID not found.");

            // TODO: Call service/repository to get user data
            var userData = _authService.GetUserDetails(userId);



            return Ok(userData.Result);
        }

    }

}


