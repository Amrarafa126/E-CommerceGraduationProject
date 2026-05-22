using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Orders
{
    public record OrderDto(Guid Id, string OrderNumber, int Status, decimal SubTotal, decimal ShippingCost, decimal TaxAmount, decimal TotalAmount, string Currency, string? BuyerNotes, string? CancellationReason, Guid BuyerId, string BuyerName, Guid SellerCompanyId, string SellerCompanyName, Guid? PaymentId, int? PaymentStatus, Guid? ShipmentId, int? ShipmentStatus, List<OrderItemDto> Items, List<OrderStatusHistoryDto> StatusHistory, DateTime CreatedAt, DateTime? UpdatedAt);
    public record OrderItemDto(Guid Id, Guid ProductId, string ProductName, string? VariantSKU, int Quantity, decimal UnitPrice, decimal LineTotal);
    public record OrderStatusHistoryDto(int Status, string Note, DateTime CreatedAt);
    public record CreateOrderDto(Guid SellerCompanyId, string? Notes, string Currency, List<CreateOrderItemDto> Items);
    public record CreateOrderItemDto(Guid ProductId, Guid? ProductVariantId, int Quantity);

    file static class OrderMapper
    {
        internal static OrderDto MapOrder(Order o) => new(
            o.Id, o.OrderNumber, (int)o.Status - 1,
            o.SubTotal, o.ShippingCost, o.TaxAmount, o.TotalAmount, o.Currency,
            o.BuyerNotes, o.CancellationReason,
            o.BuyerId, o.Buyer?.FullName ?? "",
            o.SellerCompanyId, o.SellerCompany?.CompanyName ?? "",
            o.PaymentId, o.Payment != null ? (int)o.Payment.Status - 1 : null,
            o.ShipmentId, o.Shipment != null ? (int)o.Shipment.Status - 1 : null,
            o.Items.Select(i => new OrderItemDto(
                i.Id, i.ProductId, i.ProductName, i.VariantSKU,
                i.Quantity, i.UnitPrice, i.TotalPrice)).ToList(),
            o.StatusHistory.OrderBy(h => h.CreatedAt)
                .Select(h => new OrderStatusHistoryDto(
                    (int)h.Status - 1, h.Note, h.CreatedAt)).ToList(),
            o.CreatedAt, o.UpdatedAt);
    }

     static class MapExt
    {
        internal static OrderDto MapOrder(Order o) =>
              OrderMapper.MapOrder(o);
    }
}


