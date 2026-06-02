using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Entity
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;

        // Product snapshot at order time
        public string ProductName { get; private set; } = string.Empty;
        public string? ProductMainImageUrl { get; private set; }
        public string? ProductDescription { get; private set; }
        public string? ProductCategoryName { get; private set; }
        public decimal OriginalBasePrice { get; private set; }
        public bool PriceTierApplied { get; private set; }
        public int? PriceTierMinQuantity { get; private set; }
        public string? SellerCompanyName { get; private set; }
        public string? ProductVariantName { get; private set; }
        public string? VariantSKU { get; private set; }
        public decimal? NegotiatedUnitPrice { get; private set; }

        public Guid OrderSubOrderId { get; private set; }
        public OrderSubOrder OrderSubOrder { get; private set; } = null!;

        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        public Guid? ProductVariantId { get; private set; }
        public ProductVariant? ProductVariant { get; private set; }

        private OrderItem() { }

        public static OrderItem Create(
            Guid orderSubOrderId, Guid productId, string productName,
            int quantity, decimal unitPrice,
            Guid? variantId = null, string? variantSku = null,
            string? productImage = null, string? description = null,
            string? categoryName = null, decimal originalBasePrice = 0,
            bool priceTierApplied = false, int? priceTierMinQty = null,
            string? sellerCompanyName = null, string? variantName = null,
            decimal? negotiatedPrice = null)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

            return new OrderItem
            {
                OrderSubOrderId = orderSubOrderId,
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                ProductVariantId = variantId,
                VariantSKU = variantSku,
                ProductMainImageUrl = productImage,
                ProductDescription = description,
                ProductCategoryName = categoryName,
                OriginalBasePrice = originalBasePrice,
                PriceTierApplied = priceTierApplied,
                PriceTierMinQuantity = priceTierMinQty,
                SellerCompanyName = sellerCompanyName,
                ProductVariantName = variantName,
                NegotiatedUnitPrice = negotiatedPrice
            };
        }
    }
}
