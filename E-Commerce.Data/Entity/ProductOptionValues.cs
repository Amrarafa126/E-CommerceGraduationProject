using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductOptionValue : BaseEntity
    {
        public string Value { get; private set; } = string.Empty;
        public int DisplayOrder {  get; set; } = 0;
        public Guid ProductOptionId { get; private set; }
        public ProductOption ProductOption { get; private set; } = null!;
        public ICollection<ProductVariantOptionValue> VariantOptionValues { get; private set; } = new List<ProductVariantOptionValue>();

        private ProductOptionValue() { }

        public static ProductOptionValue Create(Guid optionId, string value)
            => new()
            {
                ProductOptionId = optionId,
                Value = value.Trim(),
            };

        public void Update(string value, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.");
            Value = value.Trim();
            DisplayOrder = displayOrder;
        }
    }
}
