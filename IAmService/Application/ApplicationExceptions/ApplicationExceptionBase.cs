using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationExceptions
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class UserNotFoundException : ApplicationExceptionBase
    {
        public UserNotFoundException(string message) : base(message) { }
    }

    public class InvalidPasswordException : ApplicationExceptionBase
    {
        public InvalidPasswordException(string message) : base(message) { }
    }

    public class InvalidUserAggregateException : ApplicationExceptionBase
    {
        public InvalidUserAggregateException(string message) : base(message) { }
    }

    public class UserAlreadyExists : ApplicationExceptionBase
    {
        public UserAlreadyExists(string message) : base(message) { }
    }

    public class RoleCodeNotFound : ApplicationExceptionBase
    {
        public RoleCodeNotFound(string roleCode)
            : base($"Role with code '{roleCode}' not found.") { }
    }

    public class InvalidRole : ApplicationExceptionBase
    {
        public InvalidRole() : base("User has no permission to this role.") { }
    }

    public class InvalidTokenException : ApplicationExceptionBase
    {
        public InvalidTokenException(string message) : base(message) { }
    }
}
