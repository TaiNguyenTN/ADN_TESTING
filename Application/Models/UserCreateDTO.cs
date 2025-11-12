using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UserCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public List<string> RoleCodes { get; set; } = new List<string>();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public AddressDTO? Address { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}
