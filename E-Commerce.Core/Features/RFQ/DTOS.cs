namespace E_Commerce.Core.Features.RFQ
{
    public class RfqRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Quantity { get; set; }
        public int UnitOfMeasure { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? TargetPrice { get; set; }
        public string? ShippingCountry { get; set; }
        public string? DestinationCity { get; set; }
        public string? DestinationCountry { get; set; }
        public int? PreferredShippingMethod { get; set; }
        public string? PaymentTerms { get; set; }
        public string? RequiredCertifications { get; set; }
        public string? SupplierRequirements { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public bool IsPublic { get; set; }
        public int Status { get; set; }
        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; } = "";
        public Guid? SellerCompanyId { get; set; }
        public string? SellerCompanyName { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Attachments { get; set; }
        public List<RfqQuoteDto> Quotes { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RfqQuoteDto
    {
        public Guid Id { get; set; }
        public Guid RfqRequestId { get; set; }
        public Guid SellerCompanyId { get; set; }
        public string SellerCompanyName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public int ValidityDays { get; set; }
        public DateTime ValidUntil { get; set; }
        public int? LeadTimeDays { get; set; }
        public bool SampleAvailable { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeclined { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record CreateRfqDto(
        string Title,
        string Description,
        int Quantity,
        Guid? SellerCompanyId,
        string Currency = "EGP",
        int UnitOfMeasure = 1,
        Guid? CategoryId = null,
        string? TargetPrice = null,
        string? ShippingCountry = null,
        string? DestinationCity = null,
        string? DestinationCountry = null,
        int? PreferredShippingMethod = null,
        string? PaymentTerms = null,
        string? RequiredCertifications = null,
        string? SupplierRequirements = null,
        DateTime? DeadlineDate = null,
        Guid? ProductId = null,
        string? Attachments = null,
        bool IsPublic = true);

    public record CreateQuoteDto(
        Guid RfqRequestId,
        decimal UnitPrice,
        int Quantity,
        string Currency = "EGP",
        string? Notes = null,
        string? PaymentTerms = null,
        string? DeliveryTerms = null,
        int ValidityDays = 7,
        int? LeadTimeDays = null,
        bool SampleAvailable = false);

    public record RfqMarketplaceFilterDto(
        string? Search = null,
        Guid? CategoryId = null,
        string? Country = null,
        int? Status = null);
}
