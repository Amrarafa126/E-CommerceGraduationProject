using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record ForgotPasswordCommand(string Email)
        : IRequest<ApiResponse<object>>;
}
