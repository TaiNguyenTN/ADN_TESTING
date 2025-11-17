using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Aggregate;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private static readonly string JwtKey =
            Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY not found in enviroment variables");
        private static readonly string JwtIssuer =
            Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER not found in enviroment varbiales");
        private static readonly string JwtAudience =
            Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE not found in enviroment variables");
        private static readonly string JwtExpireMinutes =
            Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? throw new InvalidOperationException("JWT_EXPIRY_MINUTES not found in enviroment variables");
        public string GeneratePasswordResetToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Purpose", "PasswordReset"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(JwtExpireMinutes)),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtIssuer,
                ValidateAudience = true,
                ValidAudience = JwtAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                ClockSkew = TimeSpan.Zero,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                //out _ = bắt buộc phải có tham số out, nhưng bạn không dùng giá trị đó -> bỏ nó đi.
                //giúp code gọn sạch, không cần khai báo biến thừa.
                //vd: in.TryParse("123", out _); // chỉ kiểm tra parse được hay không, không cần giá trị kết quả
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
