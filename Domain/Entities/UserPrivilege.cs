using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.Entities
{
    public class UserPrivilege
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid UserPrivilegeId { get; private set; } = new Guid();
        public Guid UserId { get; private set; }
        public Guid PrivilegeId { get; private set; }
        public bool IsGranted { get; private set; }

        public Privilege? Privilege { get; private set; }
        public User? User { get; private set; }
        #endregion

        private UserPrivilege() { }
        public UserPrivilege(Guid userId, Guid privilegeId, bool isGranted)
        {
            UserId = userId;
            PrivilegeId = privilegeId;
            IsGranted = isGranted;
        }

        #region Methods
        public void UpdateGranted(bool isGranted)
        {
            IsGranted = isGranted;
        }
        #endregion
    }
}
