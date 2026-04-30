using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.ValueObjects
{
    public sealed class ContactInfo :BaseEntity
    {
        public string Email { get; }
        public string Phone { get; }

        private ContactInfo() { }

        public ContactInfo(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone is required.", nameof(phone));

            Email = email.ToLowerInvariant();
            Phone = phone;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ContactInfo other) return false;
            return Email == other.Email && Phone == other.Phone;
        }

        public override int GetHashCode() => HashCode.Combine(Email, Phone);
    }

}
