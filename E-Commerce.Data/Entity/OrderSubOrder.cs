using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class OrderSubOrder : BaseEntity
    {
        public string SubOrderNumber { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public decimal SubTotal { get; private set; }
        public decimal ShippingCost { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public string? CancellationReason { get; private set; }
        public string? PaymentTerms { get; private set; }
        public decimal? DepositAmount { get; private set; }
        public bool DepositPaid { get; private set; }
        public decimal? BalanceDue { get; private set; }
        public DateTime? DueDate { get; private set; }

        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public Guid SellerCompanyId { get; private set; }
        public Company SellerCompany { get; private set; } = null!;

        public Guid? PaymentId { get; private set; }
        public Payment? Payment { get; private set; }

        public Guid? ShipmentId { get; private set; }
        public Shipping? Shipment { get; private set; }

        public Guid? RfqQuoteId { get; private set; }

        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; private set; } = new List<OrderStatusHistory>();

        private OrderSubOrder() { }

        public static OrderSubOrder Create(
            Guid orderId, Guid sellerCompanyId,
            string currency = "EGP", string? paymentTerms = null,
            Guid? rfqQuoteId = null)
        {
            return new OrderSubOrder
            {
                OrderId = orderId,
                SellerCompanyId = sellerCompanyId,
                SubOrderNumber = GenerateSubOrderNumber(),
                Currency = currency.ToUpper(),
                PaymentTerms = paymentTerms,
                RfqQuoteId = rfqQuoteId
            };
        }

        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot add items to a non-pending sub-order.");
            Items.Add(item);
            RecalculateTotals();
        }

        public void RecalculateTotals(decimal? shippingCost = null, decimal taxRate = 0)
        {
            SubTotal = Items.Sum(i => i.TotalPrice);
            ShippingCost = shippingCost ?? ShippingCost;
            TaxAmount = Math.Round(SubTotal * taxRate, 2);
            TotalAmount = SubTotal + ShippingCost + TaxAmount;

            if (DepositAmount.HasValue)
            {
                BalanceDue = TotalAmount - DepositAmount.Value;
            }
        }

        public void SetDeposit(decimal depositAmount)
        {
            if (depositAmount < 0 || depositAmount > TotalAmount)
                throw new ArgumentException("Invalid deposit amount.");
            DepositAmount = depositAmount;
            BalanceDue = TotalAmount - depositAmount;
        }

        public void MarkDepositPaid()
        {
            DepositPaid = true;
            AddStatusHistory(OrderStatus.Paid, "Deposit paid.");
            MarkAsUpdated();
        }

        public void LinkPayment(Guid paymentId)
        {
            PaymentId = paymentId;
            MarkAsUpdated();
        }

        public void MarkPaid()
        {
            EnsureStatus(OrderStatus.Pending);
            Status = OrderStatus.Paid;
            AddStatusHistory(OrderStatus.Paid, "Payment confirmed.");
            MarkAsUpdated();
        }

        public void MarkProcessing()
        {
            EnsureStatus(OrderStatus.Paid);
            Status = OrderStatus.Processing;
            AddStatusHistory(OrderStatus.Processing, "Seller is preparing the order.");
            MarkAsUpdated();
        }

        public void MarkShipped()
        {
            EnsureStatus(OrderStatus.Processing);
            Status = OrderStatus.Shipped;
            AddStatusHistory(OrderStatus.Shipped, "Order handed to carrier.");
            MarkAsUpdated();
        }

        public void MarkDelivered()
        {
            EnsureStatus(OrderStatus.Shipped);
            Status = OrderStatus.Delivered;
            AddStatusHistory(OrderStatus.Delivered, "Order delivered to buyer.");
            MarkAsUpdated();
        }

        public void MarkCompleted()
        {
            EnsureStatus(OrderStatus.Delivered);
            Status = OrderStatus.Completed;
            AddStatusHistory(OrderStatus.Completed, "Order completed.");
            MarkAsUpdated();
        }

        public void Cancel(string reason)
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Completed)
                throw new InvalidOperationException($"Cannot cancel an order in status {Status}.");
            Status = OrderStatus.Cancelled;
            CancellationReason = reason;
            AddStatusHistory(OrderStatus.Cancelled, $"Cancelled: {reason}");
            MarkAsUpdated();
        }

        public void MarkRefunded()
        {
            Status = OrderStatus.Refunded;
            AddStatusHistory(OrderStatus.Refunded, "Payment refunded.");
            MarkAsUpdated();
        }

        public void LinkShipment(Guid shipmentId)
        {
            ShipmentId = shipmentId;
            MarkAsUpdated();
        }

        private void EnsureStatus(params OrderStatus[] allowed)
        {
            if (!allowed.Contains(Status))
                throw new InvalidOperationException(
                    $"Action not allowed in status '{Status}'. Allowed: {string.Join(", ", allowed)}");
        }

        private void AddStatusHistory(OrderStatus status, string note)
            => StatusHistory.Add(OrderStatusHistory.Create(Id, status, note));

        private static string GenerateSubOrderNumber()
        {
            var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var rand = Random.Shared.Next(100, 999);
            return $"SUB-{ts}-{rand}";
        }
    }
}
