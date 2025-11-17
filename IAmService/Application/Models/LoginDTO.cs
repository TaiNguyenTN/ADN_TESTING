using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class LoginDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set;} = string.Empty;
        public string RoleCode { get; set;} = string.Empty;
    }
}
