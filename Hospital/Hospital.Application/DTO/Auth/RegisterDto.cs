using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Auth
{
    public class RegisterDto
    {
        public string Message { get; set; }
        public bool IsRegistered { get; set; }
        public string? UserId { get; set; }
    }
}
