using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = "Patient,Admin")]
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

            // Call the register method of AuthService
            var result = await _authService.RegisterAsync(model);

            // Handle authentication result
            if (!result.IsAuthenticated)
            {
                ModelState.AddModelError("RegistrationError", result.Message);
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        [Authorize(Roles = "Doctor,Patient,Admin")]

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

    }

}
