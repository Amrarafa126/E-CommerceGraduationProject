using E_Commerce.Data.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{

    public class OrderStatusHistory : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public string Note { get; private set; } = string.Empty;

        private OrderStatusHistory() { }

        public static OrderStatusHistory Create(Guid orderId, OrderStatus status, string note)
            => new() { OrderId = orderId, Status = status, Note = note };
    }
}
