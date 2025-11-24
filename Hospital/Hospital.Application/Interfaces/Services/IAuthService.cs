using Hospital.Application.DTO.Admin;
using Hospital.Application.DTO.Auth;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<RegisterDto> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<string> AddRoleAsync(AddRoleModel mode);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<string?> GetUserIdByEmailAsync(string email);
        Task<bool> VerifyCodeAsync(string email, string code);
        Task<AdminDto> GetUserDetails(string userId);
    }
}
