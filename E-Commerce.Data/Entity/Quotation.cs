using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Quotation
    {
        public int Id { get; set; }

        public int RFQId { get; set; }
        [ForeignKey("RFQId")]
        public Rfq? RFQ { get; set; }

        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Company? Supplier { get; set; }

        public decimal Price { get; set; }

        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
