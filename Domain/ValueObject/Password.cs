using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DomainException;

namespace Domain.ValueObject
{
    public class Password
    {
        public string Hashed { get; private set; } = string.Empty;

        private Password() { }

        private Password(string hashed)
        {
            Hashed = hashed;
        }

        public static void ValidatePlain(string plain)
        {
            if(string.IsNullOrWhiteSpace(plain))
                throw new InvalidPasswordOVException("Password cannot be empty or whitespace.");

        }

        public static Password FromPlain(string plain)  //Hàm này có tác dụng là dùng để Hash mật khẩu từ dạng plain text
        {
            ValidatePlain(plain);
            var hashed = BCrypt.Net.BCrypt.HashPassword(plain);
            return new Password(hashed);

        }

        public bool Verify(string plain)  //Hàm này có tác dụng để kiểm tra mật khẩu nhập vào có đúng với mật khẩu đã hash hay không
        {
            return BCrypt.Net.BCrypt.Verify(plain, Hashed);
        }


    }
}
