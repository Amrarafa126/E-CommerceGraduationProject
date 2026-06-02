using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record ChangePasswordCommand(string CurrentPassword, string NewPassword)
        : IRequest<ApiResponse<object>>;
}
