using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Auth
{
    public class VerifyCodeDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
