using E_Commerce.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Wallet : BaseEntity
    {
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;
        public decimal PendingBalance { get; private set; }    // Awaiting release
        public decimal AvailableBalance { get; private set; }  // Ready for payout

        public string Currency { get; private set; } = "EGP"; 
        public decimal TotalEarned { get; private set; }

        private Wallet() { }

        public static Wallet Create(Guid companyId) => new()
        {
            CompanyId = companyId
        };

        public void CreditPending(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.");
            PendingBalance += amount;
            TotalEarned += amount;
            MarkAsUpdated();
        }

        public void ReleasePending(decimal amount)
        {
            if (amount > PendingBalance)
                throw new InvalidOperationException("Cannot release more than pending balance.");
            PendingBalance -= amount;
            AvailableBalance += amount;
            MarkAsUpdated();
        }

        public void DebitAvailable(decimal amount)
        {
            if (amount > AvailableBalance)
                throw new InvalidOperationException("Insufficient available balance.");
            AvailableBalance -= amount;
            MarkAsUpdated();
        }

        public decimal TotalBalance => PendingBalance + AvailableBalance;
    }

}
