using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.Entities
{
    public class UserRole
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid UserRoleId { get; private set; } = new Guid();
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public bool IsActive { get; private set; }
        
        public User? User { get; private set; }
        public Role? Role { get; private set; }
        #endregion

        private UserRole() { }

        public UserRole(Guid userId, Guid roleId, bool isActive)
        {
            UserId = userId;
            RoleId = roleId;
            IsActive = isActive;
        }

        #region
        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
        #endregion
    }
}
