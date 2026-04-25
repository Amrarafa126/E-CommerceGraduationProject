using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{

    public class ProductPriceTier : BaseEntity
    {
        public int MinQuantity { get; private set; }
        public int? MaxQuantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        private ProductPriceTier() { }
        public static ProductPriceTier Create(Guid productId, int minQty, decimal unitPrice, int? maxQty = null)
        {
            if (minQty <= 0) throw new ArgumentException("Min quantity must be > 0.");
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.");
            if (maxQty.HasValue && maxQty <= minQty)
                throw new ArgumentException("Max must be > min quantity.");

            return new ProductPriceTier
            {
                ProductId = productId,
                MinQuantity = minQty,
                UnitPrice = unitPrice,
                MaxQuantity = maxQty
            };
        }
    }
}
