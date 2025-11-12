using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationExceptions
{
    public class ApplicationException : Exception
    {
        protected ApplicationException(string message) : base(message) { }
    }

    public class UserNotFoundException : ApplicationException
    {
        public UserNotFoundException(string message) : base(message) { }
    }

    public class InvalidPasswordException : ApplicationException
    {
        public InvalidPasswordException(string message) : base(message) { }
    }

    public class InvalidUserAggregateException : ApplicationException
    {
        public InvalidUserAggregateException(string message) : base(message) { }
    }

    public class UserAlreadyExists : ApplicationException
    {
        public UserAlreadyExists(string message) : base(message) { }
    }
}
