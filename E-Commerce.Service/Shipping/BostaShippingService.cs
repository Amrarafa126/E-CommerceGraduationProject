using E_Commerce.Data.Status;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Service.Shipping
{
    public class BostaShippingService : IBostaShippingService
    {
        private readonly IBostaClient _bostaClient;
        private readonly ILogger<BostaShippingService> _logger;

        public BostaShippingService(IBostaClient bostaClient, ILogger<BostaShippingService> logger)
        {
            _bostaClient = bostaClient;
            _logger = logger;
        }

        public async Task<BostaCreateDeliveryResponse> CreateShipmentAsync(
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
            CancellationToken ct = default)
        {
            var request = new BostaCreateDeliveryRequest(
                type: codAmount > 0 ? "COD" : "PACKAGE Delivery",
                pickupAddress: new BostaAddress(
                    firstLine: pickupAddressLine1 ?? "Seller Warehouse",
                    city: pickupCity ?? city,
                    zone: pickupState ?? state),
                dropOffAddress: new BostaAddress(
                    firstLine: addressLine1,
                    city: city,
                    zone: state),
                receiverPhoneNumber: phoneNumber ?? "+201000000000",
                receiverName: recipientName,
                cod: codAmount > 0 ? codAmount.ToString("F2") : null,
                specs: new BostaSpecs(
                    packageCount: 1,
                    packageType: "Parcel",
                    packageWeight: "1"));

            return await _bostaClient.CreateDeliveryAsync(request, ct);
        }

        public Task<BostaDeliveryResponse?> TrackShipmentAsync(string trackingNumber, CancellationToken ct = default)
            => _bostaClient.GetDeliveryAsync(trackingNumber, ct);

        public Task<byte[]> GetAwbAsync(string trackingNumber, CancellationToken ct = default)
            => _bostaClient.GetAwbAsync(trackingNumber, ct);

        public ShippingStatus MapBostaStateToDomain(string bostaState)
        {
            return bostaState?.ToLowerInvariant() switch
            {
                "picked_up" or "pickup" => ShippingStatus.ReadyForPickup,
                "in_transit" or "intransit" or "hub" => ShippingStatus.InTransit,
                "out_for_delivery" or "outfordelivery" => ShippingStatus.OutForDelivery,
                "delivered" or "completed" => ShippingStatus.Delivered,
                "failed" or "cancelled" => ShippingStatus.Failed,
                "returned" or "return" => ShippingStatus.Returned,
                _ => ShippingStatus.Pending
            };
        }
    }
}
