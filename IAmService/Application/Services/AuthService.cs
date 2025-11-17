using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        #endregion

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            this.tokenService = tokenService;
            this.emailService = emailService;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _unitOfWork.GetRepository<IUserRepository>().GetByEmailAsync(dto.Email);
            if (user == null)
                throw new UserEmailNotFound(dto.Email);
            var token = tokenService.GeneratePasswordResetToken(user);
            var client = Environment.GetEnvironmentVariable("CLIENT_SIDE");
            var resetLink = $"{client}/reset-password?token={token}";
            await emailService.SendPasswordResetEmailAsync(dto.Email, resetLink);

        }

        public async Task<TokenDTO> Login(LoginDTO dto)
        {
            // Get repository
            var userRepo = _unitOfWork.GetRepository<IUserRepository>();
            var refreshTokenRepo = _unitOfWork.GetRepository<IRefreshTokenRepository>();
            var roleRepo = _unitOfWork.GetRepository<IRoleRepository>();

            #region Verify User
            //Get User
            var user = await userRepo.GetByUsernameAsync(dto.UserName);
            if (user == null)
                throw new UserNotFoundException();
            if (!user.IsActive)
                throw new UserNotFoundException();

            // Verify Password
            if (!user.Password.Verify(dto.Password))
                throw new InvalidPasswordException("Invalid password");

            //get role
            var role = await roleRepo.GetByCodeAsync(dto.RoleCode);
            if (role == null)
                throw new RoleCodeNotFound(dto.RoleCode);

            //Get roles of this user
            var userRoles = user.UserRoles;
            if (userRoles == null || !userRoles.Any())
                throw new InvalidRole();

            //Math role to user's assigned roles
            var matchedUserRole = userRoles.FirstOrDefault(
                ur => ur.RoleId == role.RoleId && ur.IsActive);
            if(matchedUserRole == null)
                throw new InvalidRole();

            //Get privileges
            var privileges = user.GetEffectivePrivileges(role.RoleId);

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

        public async Task<string> RefreshAccessTokenAsync(RefreshTokenDTO dto)
        {
            //Verify existed user
            var user = await _unitOfWork.GetRepository<IUserRepository>()
                .GetByEmailAsync(dto.Email);

            if(user == null) 
                throw new UserNotFoundException();

            //Verify refresh token
            var refreshToken = await _unitOfWork.GetRepository<IRefreshTokenRepository>()
                .GetByTokenAsync(dto.RefreshToken);

            if(refreshToken == null)
                throw new InvalidTokenException("Refresh token is expried or not found!");

            //Generate new access token
            var accessToken = JwtHelper.GenerateToken(user);

            return accessToken;
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new InvalidResetPassword("Reset password not matched.");

            var principal = tokenService.GetPrincipalFromToken(dto.ResetToken);
            if (principal == null || principal.FindFirst("Purpose")?.Value != "PasswordReset")
                throw new InvalidResetPassword("Invalid or expired token.");

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new InvalidResetPassword("Invalid token payload.");

            await _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.GetRepository<IUserRepository>().GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            user.ChangePassword(dto.NewPassword);
            await _unitOfWork.GetRepository<IUserRepository>().Update(user.UserId, user);
            await _unitOfWork.CommitAsync();

        }
    }
}
