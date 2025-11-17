using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        public string GeneratePasswordResetToken(User user);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
