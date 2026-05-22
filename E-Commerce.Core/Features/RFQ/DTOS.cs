using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ
{
    public class RfqRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Quantity { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? TargetPrice { get; set; }
        public string? ShippingCountry { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public int Status { get; set; }
        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; } = "";
        public Guid SellerCompanyId { get; set; }
        public string SellerCompanyName { get; set; } = "";
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public List<RfqQuoteDto> Quotes { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RfqQuoteDto
    {
        public Guid Id { get; set; }
        public Guid RfqRequestId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? Notes { get; set; }
        public string? PaymentTerms { get; set; }
        public string? DeliveryTerms { get; set; }
        public int ValidityDays { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeclined { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record CreateRfqDto(string Title, string Description, int Quantity, Guid SellerCompanyId, string Currency = "EGP", string? TargetPrice = null, string? ShippingCountry = null, DateTime? DeadlineDate = null, Guid? ProductId = null);
    public record CreateQuoteDto(Guid RfqRequestId, decimal UnitPrice, int Quantity, string Currency = "EGP", string? Notes = null, string? PaymentTerms = null, string? DeliveryTerms = null, int ValidityDays = 7);

}
