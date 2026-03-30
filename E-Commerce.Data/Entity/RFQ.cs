using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class Rfq
    {
        public int Id { get; set; }

        public string? BuyerId { get; set; }
        [ForeignKey("BuyerId")]
        public User? Buyer { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Quotation>? Quotations { get; set; }
    }
}
