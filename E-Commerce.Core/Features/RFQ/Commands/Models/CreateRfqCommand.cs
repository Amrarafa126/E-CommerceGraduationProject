using E_Commerce.Data.Status;
using MediatR;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record CreateRfqCommand(
        string Title,
        string Description,
        int Quantity,
        Guid? SellerCompanyId,
        string Currency,
        UnitOfMeasure UnitOfMeasure,
        Guid? CategoryId,
        string? TargetPrice,
        string? ShippingCountry,
        string? DestinationCity,
        string? DestinationCountry,
        ShippingMethod? PreferredShippingMethod,
        string? PaymentTerms,
        string? RequiredCertifications,
        string? SupplierRequirements,
        DateTime? DeadlineDate,
        Guid? ProductId,
        string? Attachments,
        bool IsPublic)
        : IRequest<ApiResponse<RfqRequestDto>>;
}
