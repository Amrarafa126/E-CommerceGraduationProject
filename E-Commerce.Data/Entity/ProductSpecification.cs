using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductSpecification : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;
        public string? GroupName { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsHighlight { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        private ProductSpecification() { }

        public static ProductSpecification Create(Guid productId, string name, string value,
            string? groupName = null, int displayOrder = 0, bool isHighlight = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specification name cannot be empty.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Specification value cannot be empty.");

            return new ProductSpecification
            {
                ProductId = productId,
                Name = name.Trim(),
                Value = value.Trim(),
                GroupName = groupName?.Trim(),
                DisplayOrder = displayOrder,
                IsHighlight = isHighlight
            };
        }

        public void Update(string name, string value, string? groupName = null,
            int displayOrder = 0, bool isHighlight = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specification name cannot be empty.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Specification value cannot be empty.");

            Name = name.Trim();
            Value = value.Trim();
            GroupName = groupName?.Trim();
            DisplayOrder = displayOrder;
            IsHighlight = isHighlight;
            MarkAsUpdated();
        }
    }
}
