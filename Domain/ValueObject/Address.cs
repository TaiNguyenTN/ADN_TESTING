using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DomainException;

namespace Domain.ValueObject
{
    public class Address
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        private Address() { }
        private Address(string street, string city, string country)
        {
            Street = street;
            City = city;
            Country = country;
        }

        public static Address Create(string street, string city, string country)
        {
            return new Address(street, city, country);
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Street))
                throw new InvalidAddressException("Street cannot be empty.");
            if (string.IsNullOrWhiteSpace(City))
                throw new InvalidAddressException("City cannot be empty.");
            if (string.IsNullOrWhiteSpace(Country))
                throw new InvalidAddressException("Country cannot be empty.");
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {Country}";
        }
    }
}
