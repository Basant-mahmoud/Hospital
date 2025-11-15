using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Helper
{
    public static class TokenHelper
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirst("uid")?.Value;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Role)?.Value;
        }
        public static string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value
                   ?? throw new InvalidOperationException("Token does not contain UserId");
        }
    }
}
