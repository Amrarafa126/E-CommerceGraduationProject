using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class OrderStatusHistory : BaseEntity
    {
        public Guid OrderSubOrderId { get; private set; }
        public OrderSubOrder OrderSubOrder { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public string Note { get; private set; } = string.Empty;

        private OrderStatusHistory() { }

        public static OrderStatusHistory Create(Guid orderSubOrderId, OrderStatus status, string note)
        {
            return new OrderStatusHistory
            {
                OrderSubOrderId = orderSubOrderId,
                Status = status,
                Note = note
            };
        }
    }
}
