using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductTag : BaseEntity
    {
        public string Tag { get; private set; } = string.Empty;
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        private ProductTag() { }

        public static ProductTag Create(Guid productId, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag cannot be empty.");

            return new ProductTag
            {
                ProductId = productId,
                Tag = tag.Trim().ToLowerInvariant()
            };
        }
    }
}
