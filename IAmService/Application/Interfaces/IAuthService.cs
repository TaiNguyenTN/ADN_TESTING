using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Domain.Aggregate;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDTO> Login(LoginDTO dto);
        Task<string> RefreshAccessTokenAsync(RefreshTokenDTO dto);
    }
}
