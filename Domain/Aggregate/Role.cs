using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DomainException;
using Domain.Entities;

namespace Domain.Aggregate
{
    public class Role
    {
        #region Attributes
        private readonly List<UserRole> userRoles = new();
        private readonly List<RolePrivilege> rolePrivileges = new();
        #endregion

        #region Properties
        public Guid RoleId { get; private set; } = new Guid();
        public string Name { get; private set; }
        public string Code { get; set; }
        public string Description { get; private set; }
        public IReadOnlyCollection<UserRole> UserRoles
        {
            get { return userRoles.AsReadOnly(); }
        }
        public IReadOnlyCollection<RolePrivilege> RolePrivileges
        {
            get { return rolePrivileges.AsReadOnly(); }
        }
        #endregion

        private Role() { }

        public Role(string name, string code, string description)
        {
            ValidateName(name);
            Name = name;
            ValidateDescription(description);
            Description = description;
            ValidateCode(code);
            Code = code;
        }

        #region Methods
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
        }

        public void AddPrivilege(Guid roleId, Guid privilegeId)
        {
            var existingPrivilege = rolePrivileges.FirstOrDefault(rp => rp.RoleId == roleId && rp.PrivilegeId == privilegeId);
            //Check xem roleId và PrivilegeId này đã tồn tại trong danh sách rolePrivileges chưa

            if (existingPrivilege != null)
            {
                if(!existingPrivilege.IsActive)
                    existingPrivilege.Activate();
                else
                    throw new InvalidRoleAggregateException("Privilege already assigned to the role.");
            }
            else
            {
                rolePrivileges.Add(new RolePrivilege(roleId, privilegeId, true));
            }
        }

        public void RemovePrivilege(Guid roleId, Guid privilegeId)
        {
            var privilege = rolePrivileges.FirstOrDefault(rp => rp.RoleId == roleId && rp.PrivilegeId == privilegeId);
            if (privilege == null || !privilege.IsActive)
                throw new InvalidRoleAggregateException("Role does not have this privilege.");

            privilege.Deactivate();
        }
        #endregion

        #region Private Validators
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidRoleAggregateException("Role name cannot be empty.");
        }

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidRoleAggregateException(
                    "Role code cannot be empty.");
        }

        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidRoleAggregateException("Role description cannot be empty.");
        }
        #endregion
    }
}
