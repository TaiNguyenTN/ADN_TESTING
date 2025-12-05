using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.Exceptions;

namespace Domain.ValueObject
{
    public class UserContract
    {
        public string? IdentityNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime Dob { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public UserContract() { }

        public UserContract(string? identityNumber, string? fullName, DateTime dob, string? gender, string? phoneNumber, string? email)
        {
            ValidateFullName(fullName);
            ValidateIdentityNumber(identityNumber);
            ValidateDateOfBirth(dob);
            ValidateGender(gender);
            ValidatePhoneNumber(phoneNumber);
            ValidateEmail(email);

            IdentityNumber = identityNumber;
            FullName = fullName;
            Dob = dob;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        #region 
        public void UpdateFullName(string? fullName)
        {
            ValidateFullName(fullName);
            FullName = fullName;
        }

        public void UpdateIdentityNumber(string? identityNumber)
        {
            ValidateIdentityNumber(identityNumber);
            IdentityNumber = identityNumber;
        }

        public void UpdateGender(string? gender)
        {
            ValidateGender(gender);
            Gender = gender;
        }

        public void UpdateDateOfBirth(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
                throw new AppointmentDomainException("Date of birth cannot be empty.");
            ValidateDateOfBirth(dateOfBirth.Value);
            Dob = dateOfBirth.Value;
        }


        public void UpdatePhoneNumber(string? phoneNumber)
        {
            ValidatePhoneNumber(phoneNumber);
            PhoneNumber = phoneNumber;
        }

        public void UpdateEmail(string? email)
        {
            ValidateEmail(email);
            Email = email;
        }
        #endregion

        #region Validation Methods
        private void ValidateFullName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new AppointmentDomainException("Full name cannot be empty.");
            if (fullName.Length < 2 || fullName.Length > 100)
                throw new AppointmentDomainException("Full name must be between 2 and 100 characters.");
        }



        private void ValidateIdentityNumber(string? identityNumber)
        {
            if (string.IsNullOrWhiteSpace(identityNumber))
                throw new AppointmentDomainException("Identity number cannot be empty.");
            if (!Regex.IsMatch(identityNumber, @"^\d{12}$"))
                throw new AppointmentDomainException("Identity number must be 12 digits.");
        }

        private void ValidateGender(string? gender)
        {
            if (gender is not null)
            {
                if (gender != GenderEnum.Male.ToString() && gender != GenderEnum.Female.ToString())
                    throw new AppointmentDomainException("GenderEnum must be either 'Male' or 'Female'.");
            }
            
        }


        private void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth > DateTime.UtcNow)
                throw new AppointmentDomainException("Date of birth cannot be in the future.");
        }



        private void ValidatePhoneNumber(string? phoneNumber)
        {
            if (phoneNumber is not null)
            {
                if (!Regex.IsMatch(phoneNumber, @"^(0|\+84)\d{9}$"))
                    throw new AppointmentDomainException("Phone number must be valid Vietnamese format (e.g., 0901234567 or +84901234567).");
            }
        }

        private void ValidateEmail(string? email)
        {
            if (email is not null)
            {
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new AppointmentDomainException("Email address format is invalid.");
            }
        }
        #endregion
    }
}
