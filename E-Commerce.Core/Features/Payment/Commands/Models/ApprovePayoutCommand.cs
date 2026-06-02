using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Models
{
    public record ApprovePayoutCommand(Guid PayoutId, string? ExternalReference = null)
        : IRequest<ApiResponse<PayoutDto>>;
}
