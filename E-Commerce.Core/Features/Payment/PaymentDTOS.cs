namespace E_Commerce.Core.Features.Payment
{
    public class PaymentDto
    {
        public Guid Id { get; init; }
        public Guid SubOrderId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "EGP";
        public int Status { get; init; }
        public int Method { get; init; }
        public string? GatewayTransactionId { get; init; }
        public string? CardLast4 { get; init; }
        public string? CardBrand { get; init; }
        public string? FailureReason { get; init; }
        public decimal AmountRefunded { get; init; }
        public DateTime? PaidAt { get; init; }
        public DateTime? RefundedAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? PaymentKey { get; init; }
    }

    public class WalletDto
    {
        public Guid CompanyId { get; init; }
        public decimal PendingBalance { get; init; }
        public decimal AvailableBalance { get; init; }
        public decimal TotalEarned { get; init; }
        public string Currency { get; init; } = "EGP";
    }

    public class PayoutDto
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "EGP";
        public int Status { get; init; }
        public string? ExternalReference { get; init; }
        public string? BankAccountLast4 { get; init; }
        public string? FailureReason { get; init; }
        public DateTime? ProcessedAt { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
