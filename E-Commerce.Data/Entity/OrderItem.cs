using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{

    public class OrderItem : BaseEntity
    {
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string ProductName { get; private set; } = string.Empty;
        public string? VariantSKU { get; private set; }

        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public Guid? ProductVariantId { get; private set; }
        public ProductVariant? ProductVariant { get; private set; }

        private OrderItem() { }

        public static OrderItem Create(
            Guid orderId, Guid productId, string productName,
            int quantity, decimal unitPrice,
            Guid? variantId = null, string? variantSku = null)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

            return new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                ProductVariantId = variantId,
                VariantSKU = variantSku
            };
        }
    }

}
