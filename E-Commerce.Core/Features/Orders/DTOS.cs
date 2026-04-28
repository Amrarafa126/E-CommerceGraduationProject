using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Orders
{
    public record OrderDto(Guid Id, string OrderNumber, string Status, decimal SubTotal, decimal ShippingCost, decimal TaxAmount, decimal TotalAmount, string Currency, string? BuyerNotes, string? CancellationReason, Guid BuyerId, string BuyerName, Guid SellerCompanyId, string SellerCompanyName, Guid? PaymentId, string? PaymentStatus, Guid? ShipmentId, string? ShipmentStatus, List<OrderItemDto> Items, List<OrderStatusHistoryDto> StatusHistory, DateTime CreatedAt, DateTime? UpdatedAt);
    public record OrderItemDto(Guid Id, Guid ProductId, string ProductName, string? VariantSKU, int Quantity, decimal UnitPrice, decimal LineTotal);
    public record OrderStatusHistoryDto(string Status, string Note, DateTime CreatedAt);
    public record CreateOrderDto(Guid SellerCompanyId, string? Notes, string Currency, List<CreateOrderItemDto> Items);
    public record CreateOrderItemDto(Guid ProductId, Guid? ProductVariantId, int Quantity);

    file static class OrderMapper
    {
        internal static OrderDto MapOrder(Order o) => new(
            o.Id, o.OrderNumber, o.Status.ToString(),
            o.SubTotal, o.ShippingCost, o.TaxAmount, o.TotalAmount, o.Currency,
            o.BuyerNotes, o.CancellationReason,
            o.BuyerId, o.Buyer?.FullName ?? "",
            o.SellerCompanyId, o.SellerCompany?.CompanyName ?? "",
            o.PaymentId, o.Payment?.Status.ToString(),
            o.ShipmentId, o.Shipment?.Status.ToString(),
            o.Items.Select(i => new OrderItemDto(
                i.Id, i.ProductId, i.ProductName, i.VariantSKU,
                i.Quantity, i.UnitPrice, i.TotalPrice)).ToList(),
            o.StatusHistory.OrderBy(h => h.CreatedAt)
                .Select(h => new OrderStatusHistoryDto(
                    h.Status.ToString(), h.Note, h.CreatedAt)).ToList(),
            o.CreatedAt, o.UpdatedAt);
    }

     static class MapExt
    {
        internal static OrderDto MapOrder(Order o) =>
              OrderMapper.MapOrder(o);
    }
}


