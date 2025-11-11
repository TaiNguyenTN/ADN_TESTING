using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        public Task<TokenDTO> Login(LoginDTO dto)
        {
            // Get repository
            throw new NotImplementedException();
        }
    }
}
