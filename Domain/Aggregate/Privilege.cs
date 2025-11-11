using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DomainException;
using Domain.Entities;

namespace Domain.Aggregate
{
    public class Privilege
    {
        #region Attributes
        private readonly List<UserPrivilege> _userPrivilege = new();
        private readonly List<RolePrivilege> _rolePrivilege = new();
        #endregion

        #region Properties
        public Guid PrivilegeId { get; private set; } = new Guid();
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public IReadOnlyCollection<UserPrivilege> UserPrivileges
        {
            get { return _userPrivilege.AsReadOnly(); }
        }

        public IReadOnlyCollection<RolePrivilege> RolePrivileges
        {
            get { return _rolePrivilege.AsReadOnly(); }
        }
        #endregion

        private Privilege() { }

        public Privilege(string name, string description)
        {
            ValidateName(name);
            ValidateDescription(description);

            Name = name;
            Description = description;
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
        #endregion

        #region Private Validators
        public static void ValidateName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new InvalidPrivilegeAggregateException("Privilege name cannot be empty.");
        } 

        public static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidPrivilegeAggregateException("Privilege description cannot be empty.");
        }
        #endregion
    }
}
