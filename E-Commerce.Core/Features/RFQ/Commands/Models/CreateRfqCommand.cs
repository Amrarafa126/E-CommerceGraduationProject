using MediatR;
using System;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record CreateRfqCommand(
     string Title, string Description, int Quantity, Guid SellerCompanyId,
     string Currency = "USD", string? TargetPrice = null,
     string? ShippingCountry = null, DateTime? DeadlineDate = null, Guid? ProductId = null, string? Attachments = null)
     : IRequest<ApiResponse<RfqRequestDto>>;
}
