using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IUserGrpcClient
    {
        Task<bool> CheckIDExist(string identityNumber);
        Task<UserDetail> GetUserDetailAsync(string identityNumber);
    }

    public class UserDetail
    {
        public string? IdentityNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
