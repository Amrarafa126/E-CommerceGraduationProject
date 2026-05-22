
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Entity
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public Guid? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }
        public ICollection<Category> SubCategories { get; private set; } = new List<Category>();
        public ICollection<Product> Products { get; private set; } = new List<Product>();

        private Category() { }

        public static Category Create(string name, string? description = null, Guid? parentId = null)
            => new()
            {
                Name = name,
                Description = description,
                ParentCategoryId = parentId
            };

        public void Update(string name, string? description = null, Guid? parentId = null)
        {
            Name = name;
            Description = description;
            ParentCategoryId = parentId;
            MarkAsUpdated();
        }
    }
}