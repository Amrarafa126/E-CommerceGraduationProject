using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ
{
    public record RfqRequestDto(Guid Id, string Title, string Description, int Quantity, string Currency, string? TargetPrice, string? ShippingCountry, DateTime? DeadlineDate, string Status, Guid BuyerId, string BuyerName, Guid SellerCompanyId, string SellerCompanyName, Guid? ProductId, string? ProductName, List<RfqQuoteDto> Quotes, DateTime CreatedAt, DateTime? UpdatedAt);
    public record RfqQuoteDto(Guid Id, Guid RfqRequestId, decimal UnitPrice, int Quantity, decimal TotalPrice, string Currency, string? Notes, string? PaymentTerms, string? DeliveryTerms, int ValidityDays, DateTime ValidUntil, bool IsAccepted, bool IsDeclined, bool IsExpired, DateTime CreatedAt);
    public record CreateRfqDto(string Title, string Description, int Quantity, Guid SellerCompanyId, string Currency = "USD", string? TargetPrice = null, string? ShippingCountry = null, DateTime? DeadlineDate = null, Guid? ProductId = null);
    public record CreateQuoteDto(Guid RfqRequestId, decimal UnitPrice, int Quantity, string Currency = "USD", string? Notes = null, string? PaymentTerms = null, string? DeliveryTerms = null, int ValidityDays = 7);

}
