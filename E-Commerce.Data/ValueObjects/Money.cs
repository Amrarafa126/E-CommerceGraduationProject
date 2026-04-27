using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.ValueObjects
{

    public sealed class Money :BaseEntity
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money() { }

        public Money(decimal amount, string currency = "EGP")
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.", nameof(amount));
            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add different currencies.");
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(decimal factor) => new(Amount * factor, Currency);

        public override string ToString() => $"{Amount:F2} {Currency}";

        public override bool Equals(object? obj)
        {
            if (obj is not Money other) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    }

}
