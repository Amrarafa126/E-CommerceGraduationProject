
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Entity
{
    public class Category
    {
        public int Id { get; set; }

        public string? NameCategory { get; set; }

      
        public ICollection<Product>? Products { get; set; }
    }
}
