using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Payment : BaseEntity
    {

        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public decimal Amount { get; set; }

        public string? Method { get; set; }

        public DateTime PaidAt { get; set; }
    }
}
