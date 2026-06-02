namespace E_Commerce.Core.Features.Shippings
{
    public class ShipmentDto
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public string RecipientName { get; init; } = string.Empty;
        public string AddressLine1 { get; init; } = string.Empty;
        public string? AddressLine2 { get; init; }
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public int Method { get; init; }
        public int Status { get; init; }
        public decimal ShippingCost { get; init; }
        public string Currency { get; init; } = "EGP";
        public DateTime? EstimatedDeliveryDate { get; init; }
        public DateTime? ActualDeliveryDate { get; init; }
        public string? CarrierName { get; init; }
        public string? TrackingNumber { get; init; }
        public string? TrackingUrl { get; init; }
        public string? BostaTrackingNumber { get; init; }
        public string? BostaShipmentId { get; init; }
        public List<ShipmentEventDto> Events { get; init; } = new();
        public DateTime CreatedAt { get; init; }
    }

    public class ShipmentEventDto
    {
        public Guid Id { get; init; }
        public int Status { get; init; }
        public string Description { get; init; } = string.Empty;
        public string? Location { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record CreateShipmentDto(
        Guid OrderId,
        string RecipientName,
        string AddressLine1,
        string City,
        string State,
        string Country,
        string PostalCode,
        int Method,
        decimal ShippingCost,
        string? AddressLine2 = null,
        string? PhoneNumber = null,
        DateTime? EstimatedDeliveryDate = null);

    public record UpdateShipmentStatusDto(
        int Status,
        string? CarrierName = null,
        string? TrackingNumber = null,
        string? TrackingUrl = null,
        string? FailureReason = null);
}
