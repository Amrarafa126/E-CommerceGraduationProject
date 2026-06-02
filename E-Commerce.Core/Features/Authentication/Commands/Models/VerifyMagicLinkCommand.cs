using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record VerifyMagicLinkCommand(string Email, string Token)
        : IRequest<ApiResponse<AuthResponseDto>>;
}
