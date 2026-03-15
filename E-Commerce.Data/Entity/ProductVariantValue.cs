using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductVariantValue
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant? Variant { get; set; }

        public int ProductOptionValueId { get; set; }
        [ForeignKey("ProductOptionValueId")]
        public ProductOptionValue? OptionValue { get; set; }
    }
}
