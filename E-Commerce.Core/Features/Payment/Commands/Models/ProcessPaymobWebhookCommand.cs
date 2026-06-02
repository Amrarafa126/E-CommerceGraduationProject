using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Models
{
    public record ProcessPaymobWebhookCommand(
        string Hmac,
        long OrderId,
        bool Success,
        string? TransactionId,
        decimal AmountCents,
        string Currency)
        : IRequest<ApiResponse<object>>;
}
