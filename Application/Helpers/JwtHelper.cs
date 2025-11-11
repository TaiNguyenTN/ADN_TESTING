using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Application.Helpers
{
    public static class JwtHelper
    {
        private static readonly string JwtKey = 
            Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY not found in enviroment variables");
        private static readonly string JwtIssuer = 
            Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER not found in enviroment varbiales");
        private static readonly string JwtAudience = 
            Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE not found in enviroment variables");
        private static readonly string JwtExpireMinutes = 
            Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? throw new InvalidOperationException("JWT_EXPIRY_MINUTES not found in enviroment variables");

        public static string GenerateToken(User user)
        {
            if(user == null) throw new ArgumentNullException(nameof(user));

            //Lấy tất cả role của user, join bằng dấu ,
            var roleNames = user.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList();
            //Dấu ! trong C# được gọi là null-forgiving operator(toán tử "bỏ qua null").
            //ur.Role là có thể null theo kiểu Role? trong class UserRole
            // nhưng ta đã lọc qua điều kiện Where(ur => ur.Role != null) nên ở Select ta có thể yên tâm rằng ur.Role sẽ không null
            //Vì vậy sử dụng dấu ! => Mình chắc chắn ur.Role không null tại thời điểm này, bỏ qua cảnh báo null.
            var roleClaim = string.Join(",", roleNames);

            //Lấy tất cả privilege của user, join bằng dấu ,
            var privilegeNames = user.UserPrivileges.Where(up => up.Privilege != null).Select(up => up.Privilege!.Name).ToList();
            var privilegeClaim = string.Join(",", privilegeNames);

            var claims = new List<Claim>
            {
                new Claim("Username", user.Username),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, roleClaim),
                new Claim("Privileges", privilegeClaim)
            };

            //Key & signing
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken
            (
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(JwtExpireMinutes)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
