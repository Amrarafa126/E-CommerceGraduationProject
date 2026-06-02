using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Models
{
    public record RejectPayoutCommand(Guid PayoutId, string Reason)
        : IRequest<ApiResponse<PayoutDto>>;
}
