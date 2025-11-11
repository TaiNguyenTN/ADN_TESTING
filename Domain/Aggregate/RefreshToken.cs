using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregate
{
    public class RefreshToken
    {
        #region Properties
        public Guid RefreshTokenID { get; set; } = new Guid();
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public User User { get; set; }
        #endregion

        private RefreshToken() { }

        public RefreshToken(Guid userId, string token, DateTime expiresAt)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
        }

        #region Methods
        public void Revoke()
        {
            IsRevoked = true;
        }
        #endregion
    }

}
