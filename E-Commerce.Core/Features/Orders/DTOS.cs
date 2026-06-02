namespace E_Commerce.Core.Features.Orders
{
    // Parent Order DTOs
    public class OrderDto
    {
        public Guid Id { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public int OverallStatus { get; init; }
        public decimal TotalAmount { get; init; }
        public string Currency { get; init; } = "EGP";
        public string? BuyerNotes { get; init; }
        public string? PoNumber { get; init; }
        public Guid BuyerId { get; init; }
        public string BuyerName { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<OrderSubOrderDto> SubOrders { get; init; } = new();
    }

    public class OrderSubOrderDto
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public string SubOrderNumber { get; init; } = string.Empty;
        public Guid BuyerId { get; init; }
        public string BuyerName { get; init; } = string.Empty;
        public int Status { get; init; }
        public decimal SubTotal { get; init; }
        public decimal ShippingCost { get; init; }
        public decimal TaxAmount { get; init; }
        public decimal TotalAmount { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string? CancellationReason { get; init; }
        public string? PaymentTerms { get; init; }
        public bool DepositPaid { get; init; }
        public decimal? BalanceDue { get; init; }
        public DateTime? DueDate { get; init; }
        public Guid SellerCompanyId { get; init; }
        public string SellerCompanyName { get; init; } = string.Empty;
        public int? PaymentStatus { get; init; }
        public int? ShipmentStatus { get; init; }
        public string? BostaTrackingNumber { get; init; }
        public string? BostaShipmentId { get; init; }
        public List<OrderItemDto> Items { get; init; } = new();
        public List<OrderStatusHistoryDto> StatusHistory { get; init; } = new();
        public DateTime CreatedAt { get; init; }
    }

    public class OrderItemDto
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? ProductMainImageUrl { get; init; }
        public string? ProductDescription { get; init; }
        public string? ProductCategoryName { get; init; }
        public decimal OriginalBasePrice { get; init; }
        public bool PriceTierApplied { get; init; }
        public int? PriceTierMinQuantity { get; init; }
        public string? SellerCompanyName { get; init; }
        public string? ProductVariantName { get; init; }
        public string? VariantSKU { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal LineTotal { get; init; }
        public decimal? NegotiatedUnitPrice { get; init; }
    }

    public class OrderStatusHistoryDto
    {
        public int Status { get; init; }
        public string Note { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public record CreateOrderDto(
        Guid SellerCompanyId,
        string? Notes,
        string Currency,
        List<CreateOrderItemDto> Items,
        string? IdempotencyKey = null,
        string? PoNumber = null);

    public record CreateOrderItemDto(
        Guid ProductId,
        Guid? ProductVariantId,
        int Quantity);
}
