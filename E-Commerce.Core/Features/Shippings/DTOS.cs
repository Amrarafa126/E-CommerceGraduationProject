using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings
{
    public record ShipmentDto(Guid Id, Guid OrderId, string RecipientName, string AddressLine1, string? AddressLine2, string City, string State, string Country, string PostalCode, string? PhoneNumber, int Method, int Status, decimal ShippingCost, string Currency, DateTime? EstimatedDeliveryDate, DateTime? ActualDeliveryDate, string? CarrierName, string? TrackingNumber, string? TrackingUrl, List<ShipmentEventDto> Events, DateTime CreatedAt);
    public record ShipmentEventDto(Guid Id, int Status, string Description, string? Location, DateTime CreatedAt);
    public record CreateShipmentDto(Guid OrderId, string RecipientName, string AddressLine1, string City, string State, string Country, string PostalCode, int Method, decimal ShippingCost, string? AddressLine2 = null, string? PhoneNumber = null, DateTime? EstimatedDeliveryDate = null);
    public record UpdateShipmentStatusDto(int Status, string? CarrierName = null, string? TrackingNumber = null, string? TrackingUrl = null, string? FailureReason = null);

    file static class ShipmentMapper
    {
        internal static ShipmentDto MapShipment(Shipping s) => new(
            s.Id, s.OrderId, s.RecipientName,
            s.AddressLine1, s.AddressLine2, s.City, s.State,
            s.Country, s.PostalCode, s.PhoneNumber,
            (int)s.Method - 1, (int)s.Status - 1,
            s.ShippingCost, s.Currency,
            s.EstimatedDeliveryDate, s.ActualDeliveryDate,
            s.CarrierName, s.TrackingNumber, s.TrackingUrl,
            s.Events.OrderBy(e => e.CreatedAt)
                .Select(e => new ShipmentEventDto(
                    e.Id, (int)e.Status - 1, e.Description, e.Location, e.CreatedAt))
                .ToList(),
            s.CreatedAt);
    }

    // Make helper accessible to commands namespace too
    
         static class ShippingMapper
        {
            internal static ShipmentDto MapShipment(Shipping s) =>
                ShipmentMapper.MapShipment(s);
        }
 }

