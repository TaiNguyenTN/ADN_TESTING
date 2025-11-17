using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.IRepositories;
using Infrastructure.Data;
using Infrastructure.InfrastructureException;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        #region Attributes
        private readonly IAMDBContext _context;
        #endregion

        public RefreshTokenRepository(IAMDBContext context)
        {
            _context = context;
        }
        public async Task<string> AddTokenAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new RepositoryException("User not found when adding refresh token.");

            //Generate new token
            string newToken = GenerateRefreshToken();

            //Check if a refresh token already exists
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

            if(existingToken != null)
            {
                existingToken.Token = newToken;
                existingToken.ExpiresAt = DateTime.UtcNow.AddDays(7); //Set lại thời gian hết hạn
                existingToken.CreateAt = DateTime.UtcNow;
                existingToken.IsRevoked = false;
            }
            else
            {
                var refreshToken = new RefreshToken(userId, newToken, DateTime.UtcNow.AddDays(7));
                await _context.RefreshTokens.AddAsync(refreshToken);
            }

            await _context.SaveChangesAsync();
            return newToken;
        }

        public async Task<string?> GetByTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);

            if (refreshToken == null)
                throw new RepositoryException("Refresh token not found or invalid.");

            return refreshToken.Token;
        }

        private string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
    }
}
