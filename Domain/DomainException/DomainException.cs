using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainException
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }

    public class InvalidPasswordOVException : DomainException
    {
        public InvalidPasswordOVException(string message) : base(message) { }
    }

    public class InvalidAddressException : DomainException
    {
        public InvalidAddressException(string message) : base(message) { }
    }

    public class InvalidUserAggregateException : DomainException
    {
        public InvalidUserAggregateException(string message) : base(message) { }
    }

    public class InvalidRoleAggregateException : DomainException
    {
        public InvalidRoleAggregateException(string message) : base(message) { }
    }

    public class InvalidPrivilegeAggregateException : DomainException
    {
        public InvalidPrivilegeAggregateException(string message) : base(message) { }
    }

}
