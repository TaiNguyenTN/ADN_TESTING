using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class AppointmentDomainException : Exception
    {
        public AppointmentDomainException() { }
        public AppointmentDomainException(string message) : base(message) { }

        public AppointmentDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
