using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword)
        : IRequest<ApiResponse<object>>;
}
