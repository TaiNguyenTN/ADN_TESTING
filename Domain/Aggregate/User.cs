using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.DomainException;
using Domain.Entities;
using Domain.Enum;
using Domain.ValueObject;

namespace Domain.Aggregate
{
    public class User
    {
        #region Attribute
        private readonly List<UserRole> userRoles = new List<UserRole>();
        private readonly List<UserPrivilege> userPrivileges = new List<UserPrivilege>();
        #endregion

        public Guid UserId { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public Password Password { get; private set; }
        public DateTime Dob { get; private set; }
        public string Gender { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public Address? Address { get; private set; }
        public bool IsActive { get; private set; } = true;
        public RefreshToken? RefreshToken { get; private set; }
        public IReadOnlyCollection<UserRole> UserRoles
        {
            get { return userRoles.AsReadOnly(); }
        }
        public IReadOnlyCollection<UserPrivilege> UserPrivileges
        {
            get { return userPrivileges.AsReadOnly(); }
        }

        private User() { }

        public User(string username, string password, DateTime dob, string gender, string phoneNumber, string email, 
            Address? address = null)
        {
            UserId = Guid.NewGuid();
            
            ValidateUsername(username);
            Username = username;
            
            Password = Password.FromPlain(password);

            ValidateDob(dob);
            Dob = dob;

            ValidateGender(gender);
            Gender = gender;

            ValidatePhoneNumber(phoneNumber);
            PhoneNumber = phoneNumber;

            ValidateEmail(email);
            Email = email;

            Address = address;
        }


        #region Methods
        public static void ValidateUserId(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidUserAggregateException("User ID cannot be empty.");
        }

        public static void ValidateUsername(string username)
        {
            if(string.IsNullOrWhiteSpace(username))
                throw new InvalidUserAggregateException("Username cannot be empty.");
        } 

        public static void ValidateDob(DateTime dob)
        {
            if(dob > DateTime.Now)
                throw new InvalidUserAggregateException("Date of birth cannot be in the future.");
        }

        public static void ValidateGender(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                throw new InvalidUserAggregateException("Gender cannot be empty.");
            if (gender != GenderEnum.MALE && gender != GenderEnum.FEMALE) 
                throw new InvalidUserAggregateException("Gender must be Male or Female.");
        }

        public static void ValidatePhoneNumber(string phoneNumber)
        {
            if(string.IsNullOrWhiteSpace(phoneNumber))
                throw new InvalidUserAggregateException("Phone number cannot be empty.");
        }

        public static void ValidateEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
                throw new InvalidUserAggregateException("Email cannot be empty.");
            if(!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new InvalidUserAggregateException("Email format is invalid.");
        }
        #endregion

        public void AddRole(Guid roleId)
        {
            var existingRole = userRoles.FirstOrDefault(ur => ur.RoleId ==  roleId);
            if(existingRole != null)
            {
                if (!existingRole.IsActive)
                    existingRole.Activate();
            }
            else
            {
                userRoles.Add(new UserRole(UserId, roleId, true));
            }
        }
    }
}
