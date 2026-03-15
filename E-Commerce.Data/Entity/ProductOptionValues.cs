using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductOptionValue
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int ProductOptionId { get; set; }
        [ForeignKey("ProductOptionId")]
        public ProductOption? Option { get; set; }
    }
}
