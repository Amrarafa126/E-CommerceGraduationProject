using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Product
    {
        public Product()
        {
                Images = new HashSet<ProductImage>();
                PriceTiers = new HashSet<ProductPriceTier>();
                Reviews = new HashSet<Review>();
                productVariants = new HashSet<ProductVariant>();
            ProductOptions = new HashSet<ProductOption>();

        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ProductImage>? Images { get; set; }
        public ICollection<ProductPriceTier>? PriceTiers { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ProductVariant>? productVariants { get; set; }
        public ICollection<ProductOption>? ProductOptions { get; set; }




    }
}
