using E_Commerce.Data.Status;

namespace E_Commerce.Service.Shipping
{
    public interface IBostaShippingService
    {
        Task<BostaCreateDeliveryResponse> CreateShipmentAsync(
            string recipientName,
            string addressLine1,
            string city,
            string state,
            string country,
            string postalCode,
            string? phoneNumber,
            decimal codAmount,
            string? pickupAddressLine1 = null,
            string? pickupCity = null,
            string? pickupState = null,
            CancellationToken ct = default);

        Task<BostaDeliveryResponse?> TrackShipmentAsync(string trackingNumber, CancellationToken ct = default);
        Task<byte[]> GetAwbAsync(string trackingNumber, CancellationToken ct = default);
        ShippingStatus MapBostaStateToDomain(string bostaState);
    }
}
