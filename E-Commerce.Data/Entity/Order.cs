using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public decimal SubTotal { get; private set; }
        public decimal ShippingCost { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; } = "USD";
        public string? BuyerNotes { get; private set; }
        public string? CancellationReason { get; private set; }

        // Relations
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;

        public Guid SellerCompanyId { get; private set; }
        public Company SellerCompany { get; private set; } = null!;

        // Payment (created after order)
        public Guid? PaymentId { get; private set; }
        public Payment? Payment { get; private set; }

        // Shipment (created after payment)
        public Guid? ShipmentId { get; private set; }
        public Shipping? Shipment { get; private set; }

        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; private set; } = new List<OrderStatusHistory>();


        private Order() { }

        public static Order Create(Guid buyerId, Guid sellerCompanyId,
            string? notes = null, string currency = "USD")
        {
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                BuyerId = buyerId,
                SellerCompanyId = sellerCompanyId,
                BuyerNotes = notes,
                Currency = currency.ToUpper()
            };
            order.AddStatusHistory(OrderStatus.Pending, "Order created.");
            return order;
        }

        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot add items to a non-pending order.");
            Items.Add(item);
            RecalculateTotals();
        }

        public void RecalculateTotals(decimal? shippingCost = null, decimal taxRate = 0)
        {
            SubTotal = Items.Sum(i => i.TotalPrice);
            ShippingCost = shippingCost ?? ShippingCost;
            TaxAmount = Math.Round(SubTotal * taxRate, 2);
            TotalAmount = SubTotal + ShippingCost + TaxAmount;
        }

        // ── Payment ───────────────────────────────────────────────────
        public void LinkPayment(Guid paymentId)
        {
            PaymentId = paymentId;
            MarkAsUpdated();
        }

        public void MarkPaid()
        {
            EnsureStatus(OrderStatus.Pending);
            Status = OrderStatus.Paid;
            AddStatusHistory(OrderStatus.Paid, "Payment confirmed.");
            MarkAsUpdated();
        }

        // ── Shipment ──────────────────────────────────────────────────
        public void LinkShipment(Guid shipmentId)
        {
            ShipmentId = shipmentId;
            MarkAsUpdated();
        }

        public void MarkProcessing()
        {
            EnsureStatus(OrderStatus.Paid);
            Status = OrderStatus.Processing;
            AddStatusHistory(OrderStatus.Processing, "Seller is preparing the order.");
            MarkAsUpdated();
        }

        public void MarkShipped()
        {
            EnsureStatus(OrderStatus.Processing);
            Status = OrderStatus.Shipped;
            AddStatusHistory(OrderStatus.Shipped, "Order handed to carrier.");
            MarkAsUpdated();
        }

        public void MarkDelivered()
        {
            EnsureStatus(OrderStatus.Shipped);
            Status = OrderStatus.Delivered;
            AddStatusHistory(OrderStatus.Delivered, "Order delivered to buyer.");
            MarkAsUpdated();
        }

        public void MarkCompleted()
        {
            EnsureStatus(OrderStatus.Delivered);
            Status = OrderStatus.Completed;
            AddStatusHistory(OrderStatus.Completed, "Order completed.");
            MarkAsUpdated();
        }

        public void Cancel(string reason)
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Completed)
                throw new InvalidOperationException($"Cannot cancel an order in status {Status}.");
            Status = OrderStatus.Cancelled;
            CancellationReason = reason;
            AddStatusHistory(OrderStatus.Cancelled, $"Cancelled: {reason}");
            MarkAsUpdated();
        }

        public void MarkRefunded()
        {
            Status = OrderStatus.Refunded;
            AddStatusHistory(OrderStatus.Refunded, "Payment refunded.");
            MarkAsUpdated();
        }

        private void EnsureStatus(params OrderStatus[] allowed)
        {
            if (!allowed.Contains(Status))
                throw new InvalidOperationException(
                    $"Action not allowed in status '{Status}'. Allowed: {string.Join(", ", allowed)}");
        }

        private void AddStatusHistory(OrderStatus status, string note)
            => StatusHistory.Add(OrderStatusHistory.Create(Id, status, note));

        private static string GenerateOrderNumber()
        {
            var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var rand = Random.Shared.Next(1000, 9999);
            return $"ORD-{ts}-{rand}";
        }
    }
}
