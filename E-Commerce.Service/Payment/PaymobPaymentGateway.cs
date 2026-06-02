using E_Commerce.Service.Interfase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace E_Commerce.Service.Payment
{
    public class PaymobPaymentGateway : IPaymentGateway
    {
        private readonly IPaymobClient _paymobClient;
        private readonly ILogger<PaymobPaymentGateway> _logger;
        private readonly PaymobOptions _options;

        public PaymobPaymentGateway(IPaymobClient paymobClient, IOptions<PaymobOptions> options, ILogger<PaymobPaymentGateway> logger)
        {
            _paymobClient = paymobClient;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<GatewayChargeResult> ChargeAsync(GatewayChargeRequest request, CancellationToken ct = default)
        {
            try
            {
                var authToken = await _paymobClient.GetAuthTokenAsync(ct);
                var amountCents = request.Amount * 100;
                var merchantOrderId = request.OrderReference;

                // Step 1: Create Paymob order
                var paymobOrderId = await _paymobClient.CreateOrderAsync(
                    authToken, amountCents, request.Currency, merchantOrderId, ct);

                // Step 2: Determine integration ID based on payment method
                var integrationId = request.PaymentMethod.ToLowerInvariant() switch
                {
                    "wallet" => _options.IntegrationIdWallet,
                    _ => _options.IntegrationIdCard
                };

                // Step 3: Generate payment key
                var paymentKey = await _paymobClient.CreatePaymentKeyAsync(
                    authToken, amountCents, request.Currency, paymobOrderId, integrationId, ct);

                _logger.LogInformation(
                    "Paymob: Created order {PaymobOrderId} with payment key for merchant order {MerchantOrderId}",
                    paymobOrderId, merchantOrderId);

                return new GatewayChargeResult(
                    Success: true,
                    TransactionId: paymobOrderId.ToString(),
                    GatewayReference: paymentKey,
                    CardLast4: null,
                    CardBrand: null,
                    ErrorCode: null,
                    ErrorMessage: null,
                    RawResponse: $"{{\"paymob_order_id\":{paymobOrderId},\"payment_key\":\"{paymentKey}\"}}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob charge failed for order {OrderRef}", request.OrderReference);
                return new GatewayChargeResult(
                    Success: false,
                    TransactionId: null,
                    GatewayReference: null,
                    CardLast4: null,
                    CardBrand: null,
                    ErrorCode: "paymob_error",
                    ErrorMessage: ex.Message,
                    RawResponse: ex.ToString());
            }
        }

        public async Task<GatewayRefundResult> RefundAsync(GatewayRefundRequest request, CancellationToken ct = default)
        {
            try
            {
                var authToken = await _paymobClient.GetAuthTokenAsync(ct);
                var amountCents = request.Amount * 100;

                var result = await _paymobClient.RefundAsync(authToken, request.TransactionId, amountCents, ct);

                if (result?.success == true)
                {
                    _logger.LogInformation("Paymob refund successful for transaction {TxnId}", request.TransactionId);
                    return new GatewayRefundResult(Success: true, RefundId: $"ref_{request.TransactionId}", ErrorMessage: null);
                }

                return new GatewayRefundResult(
                    Success: false,
                    RefundId: null,
                    ErrorMessage: "Paymob refund was not successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob refund failed for transaction {TxnId}", request.TransactionId);
                return new GatewayRefundResult(Success: false, RefundId: null, ErrorMessage: ex.Message);
            }
        }

        public async Task<GatewayPaymentStatus> GetStatusAsync(string transactionId, CancellationToken ct = default)
        {
            try
            {
                var authToken = await _paymobClient.GetAuthTokenAsync(ct);
                var result = await _paymobClient.GetTransactionAsync(authToken, transactionId, ct);

                if (result == null)
                    return GatewayPaymentStatus.Pending;

                return result.bool_response
                    ? GatewayPaymentStatus.Succeeded
                    : GatewayPaymentStatus.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob status check failed for transaction {TxnId}", transactionId);
                return GatewayPaymentStatus.Pending;
            }
        }
    }
}
