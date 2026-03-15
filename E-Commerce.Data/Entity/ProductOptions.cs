using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductOption
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public ICollection<ProductOptionValue>? Values { get; set; }
    }
}
