using MediatR;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record SubmitQuoteCommand(
        Guid RfqRequestId,
        Guid SellerCompanyId,
        decimal UnitPrice,
        int Quantity,
        string Currency = "EGP",
        string? Notes = null,
        string? PaymentTerms = null,
        string? DeliveryTerms = null,
        int ValidityDays = 7,
        int? LeadTimeDays = null,
        bool SampleAvailable = false)
        : IRequest<ApiResponse<RfqQuoteDto>>;
}
