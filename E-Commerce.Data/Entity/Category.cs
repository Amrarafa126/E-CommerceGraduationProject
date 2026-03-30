
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Entity
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
            CategoryChildren = new HashSet<Category>();
        }

        public int Id { get; set; }

        public string? NameCategory { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Category? CategoryParent { get; set; } 

        public ICollection<Category> CategoryChildren { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
