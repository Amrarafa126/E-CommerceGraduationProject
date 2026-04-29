using E_Commerce.Service.Interfase;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Repostoiry
{
    public class MockPaymentGateway(ILogger<MockPaymentGateway> logger) : IPaymentGateway
    {
        public async Task<GatewayChargeResult> ChargeAsync(
        GatewayChargeRequest request, CancellationToken ct = default)
        {
            await Task.Delay(200, ct); // Simulate network latency

            logger.LogInformation(
                "MockGateway: Charging {Amount} {Currency} for order {OrderRef}",
                request.Amount, request.Currency, request.OrderReference);

            // Simulate failure for test cards ending in 0000
            bool shouldFail = request.CardToken?.EndsWith("_fail") == true
                           || request.CardToken?.EndsWith("0000") == true;

            if (shouldFail)
            {
                return new GatewayChargeResult(
                    Success: false,
                    TransactionId: null,
                    GatewayReference: null,
                    CardLast4: null,
                    CardBrand: null,
                    ErrorCode: "card_declined",
                    ErrorMessage: "Your card was declined.",
                    RawResponse: """{"error":{"code":"card_declined","message":"Your card was declined."}}""");
            }

            // Simulate successful charge
            var txId = $"txn_{Guid.NewGuid():N}";
            var refId = $"ch_{DateTime.UtcNow:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";

            // Extract mock card details from token
            var (last4, brand) = ParseMockToken(request.CardToken);

            logger.LogInformation(
                "MockGateway: Charge successful. TxnId={TxnId}", txId);

            return new GatewayChargeResult(
                Success: true,
                TransactionId: txId,
                GatewayReference: refId,
                CardLast4: last4,
                CardBrand: brand,
                ErrorCode: null,
                ErrorMessage: null,
               RawResponse: $"""
                "id": "{txId}",
                "object": "charge",
                "amount": {(int)(request.Amount * 100)},
                "currency": "{request.Currency.ToLower()}",
                "status": "succeeded",
                "reference": "{refId}"
            
            """);
        }
        public async Task<GatewayRefundResult> RefundAsync(
            GatewayRefundRequest request, CancellationToken ct = default)
        {
            await Task.Delay(150, ct);

            logger.LogInformation(
                "MockGateway: Refunding {Amount} for transaction {TxnId}",
                request.Amount, request.TransactionId);

            var refundId = $"re_{Guid.NewGuid():N}";

            return new GatewayRefundResult(
                Success: true,
                RefundId: refundId,
                ErrorMessage: null);
        }

        public async Task<GatewayPaymentStatus> GetStatusAsync(
            string transactionId, CancellationToken ct = default)
        {
            await Task.Delay(100, ct);
            // Mock: all transactions by this gateway are succeeded
            return transactionId.StartsWith("txn_")
                ? GatewayPaymentStatus.Succeeded
                : GatewayPaymentStatus.Pending;
        }

        private static (string Last4, string Brand) ParseMockToken(string? token)
        {
            if (string.IsNullOrEmpty(token)) return ("4242", "Visa");

            // Token format: "tok_visa_4242", "tok_mc_1234" etc.
            var parts = token.Split('_');
            string brand = parts.Length > 1 ? parts[1].ToUpperInvariant() switch
            {
                "VISA" => "Visa",
                "MC" or "MASTERCARD" => "Mastercard",
                "AMEX" => "American Express",
                "MADA" => "Mada",
                _ => "Unknown"
            } : "Visa";

            string last4 = parts.Length > 2 && parts[2].Length >= 4
                ? parts[2][^4..] : "4242";

            return (last4, brand);
        }
    }
}
