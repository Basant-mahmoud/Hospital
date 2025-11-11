using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public AuthRepository(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<bool> EmailExistAsync(string email)
        {

            return await _userManager.FindByEmailAsync(email) != null;
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {

            return await _userManager.FindByNameAsync(username) != null;
        }
    }
}
