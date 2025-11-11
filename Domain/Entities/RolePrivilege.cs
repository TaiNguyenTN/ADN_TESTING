using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.Entities
{
    public class RolePrivilege
    {
        #region Properties
        public Guid RolePrivilegeId { get; private set; } = new Guid();
        public Guid RoleId { get; private set; }
        public Guid PrivilegeId { get; private set; }
        public bool IsActive { get; private set; }

        public Role? Role { get; private set; }
        public Privilege? Privilege { get; private set; }
        #endregion

        private RolePrivilege() { }

        public RolePrivilege(Guid roleId, Guid privilegeId, bool isActive)
        {
            RoleId = roleId;
            PrivilegeId = privilegeId;
            IsActive = isActive;
        }

        #region Methods
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
