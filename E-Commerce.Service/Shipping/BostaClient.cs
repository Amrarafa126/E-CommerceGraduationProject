using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace E_Commerce.Service.Shipping
{
    public interface IBostaClient
    {
        Task<BostaCreateDeliveryResponse> CreateDeliveryAsync(BostaCreateDeliveryRequest request, CancellationToken ct = default);
        Task<BostaDeliveryResponse?> GetDeliveryAsync(string trackingNumber, CancellationToken ct = default);
        Task<byte[]> GetAwbAsync(string trackingNumber, CancellationToken ct = default);
    }

    public class BostaClient : IBostaClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BostaClient> _logger;
        private readonly BostaOptions _options;

        public BostaClient(HttpClient httpClient, IOptions<BostaOptions> options, ILogger<BostaClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_options.ApiKey);
        }

        public async Task<BostaCreateDeliveryResponse> CreateDeliveryAsync(
            BostaCreateDeliveryRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v2/deliveries", request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Bosta create delivery failed: {Status} {Content}",
                    response.StatusCode, content);
                throw new HttpRequestException($"Bosta API error: {response.StatusCode} - {content}");
            }

            var result = JsonSerializer.Deserialize<BostaCreateDeliveryResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                throw new InvalidOperationException("Failed to parse Bosta create delivery response.");

            _logger.LogInformation("Bosta delivery created: {TrackingNumber}", result.trackingNumber);
            return result;
        }

        public async Task<BostaDeliveryResponse?> GetDeliveryAsync(
            string trackingNumber, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"/api/v2/deliveries/{trackingNumber}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<BostaDeliveryResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<byte[]> GetAwbAsync(string trackingNumber, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"/api/v2/deliveries/{trackingNumber}/awb", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(ct);
        }
    }

    // Request/Response DTOs
    public record BostaCreateDeliveryRequest(
        string type,
        BostaAddress pickupAddress,
        BostaAddress dropOffAddress,
        string? receiverPhoneNumber,
        string? receiverName,
        string? cod,
        BostaSpecs? specs);

    public record BostaAddress(
        string firstLine,
        string city,
        string zone);

    public record BostaSpecs(
        int packageCount,
        string packageType,
        string packageWeight);

    public record BostaCreateDeliveryResponse(
        string _id,
        string trackingNumber,
        string state);

    public record BostaDeliveryResponse(
        string _id,
        string trackingNumber,
        string state,
        string type,
        BostaAddress? pickupAddress,
        BostaAddress? dropOffAddress);
}
