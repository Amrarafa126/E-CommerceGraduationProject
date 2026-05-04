using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class Payment : BaseEntity
    {

        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; private set; }

        public string? GatewayTransactionId { get; private set; }
        public string? GatewayReference { get; private set; }
        public string? GatewayRawResponse { get; private set; }
        public string? FailureReason { get; private set; }

        public decimal AmountRefunded { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public DateTime? RefundedAt { get; private set; }

        public string? CardLast4 { get; private set; }
        public string? CardBrand { get; private set; }

        private Payment() { }

        public static Payment Create(Guid orderId, decimal amount,
            PaymentMethod method, string currency = "EGP")
        {
            if (amount <= 0) throw new ArgumentException("Payment amount must be positive.");
            return new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Currency = currency.ToUpper()
            };
        }

        public void MarkPaid(string gatewayTransactionId, string? gatewayRef = null,
            string? cardLast4 = null, string? cardBrand = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Cannot mark payment as paid from state {Status}.");

            Status = PaymentStatus.Paid;
            GatewayTransactionId = gatewayTransactionId;
            GatewayReference = gatewayRef;
            CardLast4 = cardLast4;
            CardBrand = cardBrand;
            PaidAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void MarkFailed(string reason, string? rawResponse = null)
        {
            Status = PaymentStatus.Failed;
            FailureReason = reason;
            GatewayRawResponse = rawResponse;
            MarkAsUpdated();
        }

        public void Refund(decimal refundAmount)
        {
            if (Status != PaymentStatus.Paid)
                throw new InvalidOperationException("Can only refund a paid payment.");
            if (refundAmount > Amount - AmountRefunded)
                throw new ArgumentException("Refund exceeds remaining amount.");

            AmountRefunded += refundAmount;
            Status = AmountRefunded >= Amount
                ? PaymentStatus.Refunded
                : PaymentStatus.PartiallyRefunded;
            RefundedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public bool IsPaid => Status == PaymentStatus.Paid;
        public decimal RemainingAmount => Amount - AmountRefunded;
    }
}
