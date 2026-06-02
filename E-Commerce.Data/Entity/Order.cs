using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; private set; } = string.Empty;
        public OrderOverallStatus OverallStatus { get; private set; } = OrderOverallStatus.Pending;
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public string? BuyerNotes { get; private set; }
        public string? PoNumber { get; private set; }
        public string? IdempotencyKey { get; private set; }

        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;

        public ICollection<OrderSubOrder> SubOrders { get; private set; } = new List<OrderSubOrder>();

        private Order() { }

        public static Order Create(Guid buyerId, string? notes = null, string currency = "EGP", string? poNumber = null)
        {
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                BuyerId = buyerId,
                BuyerNotes = notes,
                Currency = currency.ToUpper(),
                PoNumber = poNumber
            };
            return order;
        }

        public void SetIdempotencyKey(string key)
        {
            IdempotencyKey = key;
        }

        public void AddSubOrder(OrderSubOrder subOrder)
        {
            SubOrders.Add(subOrder);
            RecalculateTotals();
        }

        public void RecalculateTotals()
        {
            TotalAmount = SubOrders.Sum(s => s.TotalAmount);
            UpdateOverallStatus();
            MarkAsUpdated();
        }

        public void UpdateOverallStatus()
        {
            if (!SubOrders.Any())
            {
                OverallStatus = OrderOverallStatus.Pending;
                return;
            }

            // If all completed -> Completed
            if (SubOrders.All(s => s.Status == OrderStatus.Completed))
            {
                OverallStatus = OrderOverallStatus.Completed;
                return;
            }

            // If any cancelled -> PartiallyCancelled
            if (SubOrders.Any(s => s.Status == OrderStatus.Cancelled))
            {
                OverallStatus = SubOrders.All(s => s.Status == OrderStatus.Cancelled)
                    ? OrderOverallStatus.Cancelled
                    : OrderOverallStatus.PartiallyCancelled;
                return;
            }

            // If any refunded -> PartiallyRefunded
            if (SubOrders.Any(s => s.Status == OrderStatus.Refunded))
            {
                OverallStatus = SubOrders.All(s => s.Status == OrderStatus.Refunded)
                    ? OrderOverallStatus.Refunded
                    : OrderOverallStatus.PartiallyRefunded;
                return;
            }

            // If any delivered -> Delivered (but not all completed)
            if (SubOrders.Any(s => s.Status == OrderStatus.Delivered))
            {
                OverallStatus = OrderOverallStatus.Delivered;
                return;
            }

            // If any shipped -> Shipped
            if (SubOrders.Any(s => s.Status == OrderStatus.Shipped))
            {
                OverallStatus = OrderOverallStatus.Shipped;
                return;
            }

            // If any processing -> Processing
            if (SubOrders.Any(s => s.Status == OrderStatus.Processing))
            {
                OverallStatus = OrderOverallStatus.Processing;
                return;
            }

            // If any paid -> Paid
            if (SubOrders.Any(s => s.Status == OrderStatus.Paid))
            {
                OverallStatus = OrderOverallStatus.Paid;
                return;
            }

            OverallStatus = OrderOverallStatus.Pending;
        }

        public bool CanCancel => SubOrders.All(s =>
            s.Status == OrderStatus.Pending || s.Status == OrderStatus.Paid);

        private static string GenerateOrderNumber()
        {
            var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var rand = Random.Shared.Next(1000, 9999);
            return $"ORD-{ts}-{rand}";
        }
    }
}
