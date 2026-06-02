using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{

    public class ProductVariant : BaseEntity
    {
        public string SKU { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? Barcode { get; private set; }
        public string? ImageUrl { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public ICollection<ProductVariantOptionValue> OptionValues { get; private set; } = new List<ProductVariantOptionValue>();

        private ProductVariant() { }

        public static ProductVariant Create(Guid productId, string sku, decimal price,
            int stock, string? barcode = null, string? imageUrl = null)
        {
            if (price < 0) throw new ArgumentException("Price cannot be negative.");
            if (stock < 0) throw new ArgumentException("Stock cannot be negative.");
            return new ProductVariant
            {
                ProductId = productId,
                SKU = sku,
                Price = price,
                StockQuantity = stock,
                Barcode = barcode,
                ImageUrl = imageUrl,
            };
        }
        public void UpdateSku(string sku)
        {
            SKU = sku;
            MarkAsUpdated();
        }
        public void UpdateStock(int qty) { StockQuantity = qty; MarkAsUpdated(); }
        public void AdjustStock(int delta)
        {
            if (StockQuantity + delta < 0) throw new InvalidOperationException("Insufficient stock.");
            StockQuantity += delta; MarkAsUpdated();
        }
        public void UpdatePrice(decimal price) { Price = price; MarkAsUpdated(); }
        public void UpdateBarcode(string? barcode) { Barcode = barcode; MarkAsUpdated(); }
        public void UpdateImageUrl(string? imageUrl) { ImageUrl = imageUrl; MarkAsUpdated(); }
        public void Activate() { IsActive = true; MarkAsUpdated(); }
        public void Deactivate() { IsActive = false; MarkAsUpdated(); }
    }
}
