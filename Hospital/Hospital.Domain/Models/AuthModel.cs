using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string name { get; set; }
        public string passward { get; set; }

        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
