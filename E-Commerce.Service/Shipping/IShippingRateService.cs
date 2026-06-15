using E_Commerce.Data.Status;

namespace E_Commerce.Service.Shipping
{
    public interface IShippingRateService
    {
        Task<ShippingRateEstimate> EstimateAsync(
            string country,
            string city,
            string? state,
            string pickupCity,
            string? pickupState,
            ShippingMethod method,
            CancellationToken ct = default);
    }
}
