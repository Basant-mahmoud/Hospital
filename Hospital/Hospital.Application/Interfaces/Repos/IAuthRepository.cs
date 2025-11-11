using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistAsync(string email);
        Task<bool> UsernameExistsAsync(string username);

    }
}
