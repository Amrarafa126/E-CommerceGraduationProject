using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
