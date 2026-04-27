using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.ValueObjects
{
    public sealed class Address :BaseEntity
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string PostalCode { get; }

        private Address() { }

        public Address(string street, string city, string state, string country, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street is required.", nameof(street));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.", nameof(country));

            Street = street;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
        }

        public override string ToString() => $"{Street}, {City}, {State}, {Country} {PostalCode}";

        public override bool Equals(object? obj)
        {
            if (obj is not Address other) return false;
            return Street == other.Street && City == other.City &&
                   State == other.State && Country == other.Country &&
                   PostalCode == other.PostalCode;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Street, City, State, Country, PostalCode);
    }
}
