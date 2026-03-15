using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Shipping
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public string? ShippingCompany { get; set; }

        public string? TrackingNumber { get; set; }

        public DateTime ShippingDate { get; set; }
    }
}
