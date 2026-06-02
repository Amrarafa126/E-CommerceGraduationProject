using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record VerifyEmailCommand(Guid UserId, string Token)
        : IRequest<ApiResponse<object>>;
}
