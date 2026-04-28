using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductOption : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public int DisplayOrder { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public ICollection<ProductOptionValue> Values { get; private set; } = new List<ProductOptionValue>();

        private ProductOption() { }

        public static ProductOption Create(Guid productId, string name, int order = 0)
            => new() { ProductId = productId, Name = name.Trim(), DisplayOrder = order };

        public void Update(string name, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Option name cannot be empty.");

            Name = name.Trim();
            DisplayOrder = displayOrder;
        }
    }
}

