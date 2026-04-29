using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Interfase
{
    public interface IPaymentGateway
    {
        Task<GatewayChargeResult> ChargeAsync(GatewayChargeRequest request, CancellationToken ct = default);
        Task<GatewayRefundResult> RefundAsync(GatewayRefundRequest request, CancellationToken ct = default);
        Task<GatewayPaymentStatus> GetStatusAsync(string transactionId, CancellationToken ct = default);
    }

    public record GatewayChargeRequest(
        decimal Amount,
        string Currency,
        string PaymentMethod,     // "card", "wallet", etc.
        string? CardToken,        // tokenized card from client SDK
        string? CustomerId,
        string OrderReference,
        string Description,
        Dictionary<string, string>? Metadata = null);

    public record GatewayChargeResult(
        bool Success,
        string? TransactionId,
        string? GatewayReference,
        string? CardLast4,
        string? CardBrand,
        string? ErrorCode,
        string? ErrorMessage,
        string? RawResponse = null);

    public record GatewayRefundRequest(
        string TransactionId,
        decimal Amount,
        string Reason);

    public record GatewayRefundResult(
        bool Success,
        string? RefundId,
        string? ErrorMessage);

    public enum GatewayPaymentStatus { Pending, Succeeded, Failed, Refunded }

}
