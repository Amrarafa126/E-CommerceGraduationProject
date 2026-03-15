using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public string? SKU { get; set; }
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<ProductVariantValue>? VariantValues { get; set; }
    }
}
