using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record RequestChangeEmailCommand(string NewEmail)
        : IRequest<ApiResponse<object>>;
}
