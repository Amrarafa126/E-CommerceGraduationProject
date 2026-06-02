using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace E_Commerce.Service.Payment
{
    public interface IPaymobClient
    {
        Task<string> GetAuthTokenAsync(CancellationToken ct = default);
        Task<long> CreateOrderAsync(string authToken, decimal amountCents, string currency, string merchantOrderId, CancellationToken ct = default);
        Task<string> CreatePaymentKeyAsync(string authToken, decimal amountCents, string currency, long orderId, int integrationId, CancellationToken ct = default);
        Task<PaymobRefundResponse?> RefundAsync(string authToken, string transactionId, decimal amountCents, CancellationToken ct = default);
        Task<PaymobTransactionResponse?> GetTransactionAsync(string authToken, string transactionId, CancellationToken ct = default);
        bool VerifyWebhookHmac(string hmacSecret, Dictionary<string, string> payload, string receivedHmac);
    }

    public class PaymobClient : IPaymobClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymobClient> _logger;
        private readonly PaymobOptions _options;
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public PaymobClient(HttpClient httpClient, IOptions<PaymobOptions> options, ILogger<PaymobClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }

        public async Task<string> GetAuthTokenAsync(CancellationToken ct = default)
        {
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken;

            var response = await _httpClient.PostAsJsonAsync("/api/auth/tokens",
                new { api_key = _options.ApiKey }, ct);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaymobAuthResponse>(ct);

            if (string.IsNullOrEmpty(result?.token))
                throw new InvalidOperationException("Failed to obtain Paymob auth token.");

            _cachedToken = result.token;
            _tokenExpiry = DateTime.UtcNow.AddMinutes(55); // Tokens valid for ~1 hour
            return _cachedToken;
        }

        public async Task<long> CreateOrderAsync(string authToken, decimal amountCents, string currency,
            string merchantOrderId, CancellationToken ct = default)
        {
            var payload = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)amountCents,
                currency = currency.ToUpper(),
                merchant_order_id = merchantOrderId,
                items = new List<object>()
            };

            var response = await _httpClient.PostAsJsonAsync("/api/ecommerce/orders", payload, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaymobOrderResponse>(ct);

            if (result?.id == null)
                throw new InvalidOperationException("Failed to create Paymob order.");

            _logger.LogInformation("Paymob order created: {OrderId} for merchant order {MerchantOrderId}",
                result.id, merchantOrderId);

            return result.id;
        }

        public async Task<string> CreatePaymentKeyAsync(string authToken, decimal amountCents, string currency,
            long orderId, int integrationId, CancellationToken ct = default)
        {
            var payload = new
            {
                auth_token = authToken,
                amount_cents = (int)amountCents,
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = "customer@example.com",
                    floor = "NA",
                    first_name = "Customer",
                    street = "NA",
                    building = "NA",
                    phone_number = "+201000000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "Customer",
                    state = "Cairo"
                },
                currency = currency.ToUpper(),
                integration_id = integrationId,
                lock_order_when_paid = false
            };

            var response = await _httpClient.PostAsJsonAsync("/api/acceptance/payment_keys", payload, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>(ct);

            if (string.IsNullOrEmpty(result?.token))
                throw new InvalidOperationException("Failed to create Paymob payment key.");

            return result.token;
        }

        public async Task<PaymobRefundResponse?> RefundAsync(string authToken, string transactionId,
            decimal amountCents, CancellationToken ct = default)
        {
            var payload = new
            {
                auth_token = authToken,
                amount_cents = (int)amountCents,
                transaction_id = transactionId
            };

            var response = await _httpClient.PostAsJsonAsync("/api/acceptance/void_refund/refund", payload, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymobRefundResponse>(ct);
        }

        public async Task<PaymobTransactionResponse?> GetTransactionAsync(string authToken, string transactionId,
            CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/acceptance/transactions/{transactionId}");
            request.Headers.Add("Authorization", authToken);
            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymobTransactionResponse>(ct);
        }

        public bool VerifyWebhookHmac(string hmacSecret, Dictionary<string, string> payload, string receivedHmac)
        {
            // Paymob HMAC concatenation order (all values as strings, no separators)
            var keysInOrder = new[]
            {
                "amount_cents", "created_at", "currency", "delivery_fees",
                "id", "integration_id", "is_3d", "is_auth", "is_capture",
                "is_refunded", "is_standalone", "is_voided", "order.id",
                "owner", "pending", "source_data.pan", "source_data.sub_type",
                "source_data.type", "success"
            };

            var concatenated = new StringBuilder();
            foreach (var key in keysInOrder)
            {
                if (payload.TryGetValue(key, out var value))
                    concatenated.Append(value);
            }

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated.ToString()));
            var computedHmac = Convert.ToHexString(hash).ToLowerInvariant();

            return computedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase);
        }
    }

    // Response DTOs
    public record PaymobAuthResponse(string token);
    public record PaymobOrderResponse(long id);
    public record PaymobPaymentKeyResponse(string token);
    public record PaymobRefundResponse(bool success, int amount_cents, object? transaction);
    public record PaymobTransactionResponse(
        int id, string created_at, bool bool_response, string txn_response_code,
        object? acq_response_code, string source_data_sub_type,
        string source_data_pan, string data_message,
        string source_data_type, string hmac, string merchant_order_id,
        string order_id, decimal amount_cents, string currency);
}
