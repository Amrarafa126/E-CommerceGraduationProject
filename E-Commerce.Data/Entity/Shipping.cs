using E_Commerce.Data.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Shipping : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        // Destination
        public string RecipientName { get; private set; } = string.Empty;
        public string AddressLine1 { get; private set; } = string.Empty;
        public string? AddressLine2 { get; private set; }
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }

        // Shipping details
        public ShippingMethod Method { get; private set; }
        public ShippingStatus Status { get; private set; } = ShippingStatus.Pending;
        public decimal ShippingCost { get; private set; }
        public string Currency { get; private set; } = "USD";
        public DateTime? EstimatedDeliveryDate { get; private set; }
        public DateTime? ActualDeliveryDate { get; private set; }

        // Carrier tracking
        public string? CarrierName { get; private set; }
        public string? TrackingNumber { get; private set; }
        public string? TrackingUrl { get; private set; }

        public ICollection<ShippingEvent> Events { get; private set; } = new List<ShippingEvent>();

        private Shipping() { }

        public static Shipping Create(
            Guid orderId,
            string recipientName,
            string addressLine1,
            string city,
            string state,
            string country,
            string postalCode,
            ShippingMethod method,
            decimal shippingCost,
            string currency = "USD",
            string? addressLine2 = null,
            string? phone = null,
            DateTime? estimatedDelivery = null)
        {
            if (shippingCost < 0) throw new ArgumentException("Shipping cost cannot be negative.");

            return new Shipping
            {
                OrderId = orderId,
                RecipientName = recipientName,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                State = state,
                Country = country,
                PostalCode = postalCode,
                PhoneNumber = phone,
                Method = method,
                ShippingCost = shippingCost,
                Currency = currency.ToUpper(),
                EstimatedDeliveryDate = estimatedDelivery
                    ?? CalculateEstimatedDelivery(method)
            };
        }

        private static DateTime CalculateEstimatedDelivery(ShippingMethod method) =>
            method switch
            {
                ShippingMethod.Overnight => DateTime.UtcNow.AddDays(1),
                ShippingMethod.Express => DateTime.UtcNow.AddDays(3),
                ShippingMethod.Standard => DateTime.UtcNow.AddDays(7),
                ShippingMethod.Pickup => DateTime.UtcNow.AddHours(2),
                _ => DateTime.UtcNow.AddDays(7)
            };

        // ── State transitions ─────────────────────────────────────────
        public void MarkReadyForPickup()
        {
            EnsureState(ShippingStatus.Pending);
            Status = ShippingStatus.ReadyForPickup;
            AddEvent("Ready for pickup by carrier.");
            MarkAsUpdated();
        }

        public void MarkInTransit(string carrierName, string trackingNumber, string? trackingUrl = null)
        {
            EnsureState(ShippingStatus.Pending, ShippingStatus.ReadyForPickup);
            Status = ShippingStatus.InTransit;
            CarrierName = carrierName;
            TrackingNumber = trackingNumber;
            TrackingUrl = trackingUrl;
            AddEvent($"Shipped via {carrierName}. Tracking: {trackingNumber}");
            MarkAsUpdated();
        }

        public void MarkOutForDelivery()
        {
            EnsureState(ShippingStatus.InTransit);
            Status = ShippingStatus.OutForDelivery;
            AddEvent("Out for delivery.");
            MarkAsUpdated();
        }

        public void MarkDelivered()
        {
            EnsureState(ShippingStatus.InTransit, ShippingStatus.OutForDelivery);
            Status = ShippingStatus.Delivered;
            ActualDeliveryDate = DateTime.UtcNow;
            AddEvent("Delivered successfully.");
            MarkAsUpdated();
        }

        public void MarkFailed(string reason)
        {
            Status = ShippingStatus.Failed;
            AddEvent($"Delivery failed: {reason}");
            MarkAsUpdated();
        }

        public void MarkReturned()
        {
            Status = ShippingStatus.Returned;
            AddEvent("Package returned to sender.");
            MarkAsUpdated();
        }

        private void EnsureState(params ShippingStatus[] allowed)
        {
            if (!allowed.Contains(Status))
                throw new InvalidOperationException(
                    $"Cannot transition from {Status}. Allowed: {string.Join(", ", allowed)}");
        }

        private void AddEvent(string description)
            => Events.Add(ShippingEvent.Create(Id, Status, description));

        public string FullAddress =>
            $"{AddressLine1}{(AddressLine2 != null ? ", " + AddressLine2 : "")}, {City}, {State} {PostalCode}, {Country}";
    }
}
