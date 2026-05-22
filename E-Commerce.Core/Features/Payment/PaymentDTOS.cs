using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment
{
    public record PaymentDto(Guid Id, Guid OrderId, decimal Amount, string Currency, int Status, int Method, string? GatewayTransactionId, string? CardLast4, string? CardBrand, string? FailureReason, decimal AmountRefunded, DateTime? PaidAt, DateTime? RefundedAt, DateTime CreatedAt);
    public record WalletDto(Guid CompanyId, decimal PendingBalance, decimal AvailableBalance, decimal TotalEarned, string Currency);
    public record PayoutDto(Guid Id, Guid CompanyId, decimal Amount, string Currency, int Status, string? ExternalReference, string? BankAccountLast4, string? FailureReason, DateTime? ProcessedAt, DateTime CreatedAt);

}
