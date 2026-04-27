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
        public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Unpaid;
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public string? Notes { get; private set; }
        public string? ShippingAddress { get; private set; }

        // Navigation
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;
        public Guid SupplierId { get; private set; }
        public Company Supplier { get; private set; } = null!;
        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

        private Order() { }

        public static Order Create(Guid buyerId, Guid supplierId, string? shippingAddress = null, string? notes = null)
        {
            return new Order
            {
                OrderNumber = GenerateOrderNumber(),
                BuyerId = buyerId,
                SupplierId = supplierId,
                ShippingAddress = shippingAddress,
                Notes = notes
            };
        }

        private static string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
            RecalculateTotal();
        }

        public void RecalculateTotal()
        {
            TotalAmount = Items.Sum(i => i.UnitPrice * i.Quantity);
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be confirmed.");
            Status = OrderStatus.Confirmed;
            MarkAsUpdated();
        }

        public void Cancel()
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel shipped or delivered orders.");
            Status = OrderStatus.Cancelled;
            MarkAsUpdated();
        }

        public void MarkAsPaid()
        {
            PaymentStatus = PaymentStatus.Paid;
            MarkAsUpdated();
        }
    }
}
