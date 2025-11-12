using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ApplicationExceptions;
using Application.Interfaces;
using Application.Models;
using Domain.Aggregate;
using Domain.IRepositories;
using Domain.ValueObject;

namespace Application.Services
{
    public class UserService : IUserService
    {
        #region Attributes
        private readonly IUnitOfWork _unitOfWork;
        #endregion
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<UserDTO> CreateUserAsync(UserCreateDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingByEmail = await _unitOfWork.GetRepository<IUserRepository>().GetByEmailAsync(dto.Email);
            if (existingByEmail != null)
                throw new UserAlreadyExists("Email already registered.");

            var existingByPhoneNumber = await _unitOfWork.GetRepository<IUserRepository>().GetByPhoneAsync(dto.PhoneNumber);
            if (existingByPhoneNumber != null)
                throw new UserAlreadyExists("Phone number already registered.");
            //Thiếu validation

            var address = Address.Create(
            dto.Address?.Street ?? string.Empty,
            dto.Address?.City ?? string.Empty,
            dto.Address?.Country ?? string.Empty);

            var user = new User(
                username: dto.UserName,
                password: dto.Password,
                dob: dto.Dob,
                gender: dto.Gender,
                phoneNumber: dto.PhoneNumber,
                email: dto.Email,
                address: address);

            foreach(var code in dto.RoleCodes)
            {
                var role = await _unitOfWork.GetRepository<IRoleRepository>().GetByCodeAsync(code);
                if(role == null) 
                    throw new InvalidUserAggregateException($"Role with code {code} does not exist.");
                user.AddRole(role.RoleId);
            }

            _unitOfWork.GetRepository<IUserRepository>().AddAsync(user);
            await _unitOfWork.CommitAsync();

            return new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.Dob,
                Address = user.Address is null ? null : new AddressDTO
                {
                    Street = user.Address.Street,
                    City = user.Address.City,
                    Country = user.Address.Country
                }
            };

        }
    }
}
