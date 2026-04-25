using E_Commerce.Data.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Payout : BaseEntity
    {
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public PayoutStatus Status { get; private set; } = PayoutStatus.Pending;
        public string? ExternalReference { get; private set; }
        public string? BankAccountLast4 { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        private Payout() { }

        public static Payout Create(Guid companyId, decimal amount,
            string currency = "USD", string? bankLast4 = null) => new()
            {
                CompanyId = companyId,
                Amount = amount,
                Currency = currency,
                BankAccountLast4 = bankLast4
            };

        public void MarkProcessing(string externalRef)
        {
            Status = PayoutStatus.Processing;
            ExternalReference = externalRef;
            MarkAsUpdated();
        }

        public void MarkCompleted()
        {
            Status = PayoutStatus.Completed;
            ProcessedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void MarkFailed(string reason)
        {
            Status = PayoutStatus.Failed;
            FailureReason = reason;
            MarkAsUpdated();
        }
    }
}
