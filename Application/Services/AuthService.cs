using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ApplicationExceptions;
using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Domain.Aggregate;
using Domain.IRepositories;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        #region Attributes
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        #endregion

        public AuthService(IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository)
        {
            _unitOfWork = unitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<TokenDTO> Login(LoginDTO dto)
        {
            // Get repository
            var userRepo = _unitOfWork.GetRepository<IUserRepository>();
            var refreshTokenRepo = _unitOfWork.GetRepository<IRefreshTokenRepository>();

            #region Verify User
            //Get User
            var user = await userRepo.GetByUsernameAsync(dto.UserName);
            if(user == null)
                throw new UserNotFoundException("User not found");
            if(!user.IsActive)
                throw new UserNotFoundException("User not found");

            // Verify Password
            if(!user.Password.Verify(dto.Password))
                throw new InvalidPasswordException("Invalid password");

            // Generate access token
            var accessToken = JwtHelper.GenerateToken(user);

            //Generate refresh token
            await _unitOfWork.BeginTransactionAsync();
            var refreshToken = await refreshTokenRepo.AddTokenAsync(user.UserId);
            await _unitOfWork.CommitAsync(user.Username);

            //Return tokens
            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            #endregion
        }


    }
}
