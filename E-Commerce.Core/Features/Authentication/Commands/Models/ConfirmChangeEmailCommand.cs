using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record ConfirmChangeEmailCommand(Guid UserId, string NewEmail, string Token)
        : IRequest<ApiResponse<object>>;
}
