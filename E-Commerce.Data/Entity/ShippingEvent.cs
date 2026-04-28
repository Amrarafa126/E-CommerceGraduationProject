using E_Commerce.Data.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ShippingEvent : BaseEntity
    {
        public Guid ShipmentId { get; private set; }
        public Shipping Shipment { get; private set; } = null!;
        public ShippingStatus Status { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public string? Location { get; private set; }

        private ShippingEvent() { }

        public static ShippingEvent Create(Guid shipmentId, ShippingStatus status,
            string description, string? location = null)
            => new()
            {
                ShipmentId = shipmentId,
                Status = status,
                Description = description,
                Location = location
            };
    }
}
