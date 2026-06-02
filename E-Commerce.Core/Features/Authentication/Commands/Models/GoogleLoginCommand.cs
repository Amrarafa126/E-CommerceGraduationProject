using E_Commerce.Core.Features.Authentication;
using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public class GoogleLoginCommand(string idToken) : IRequest<ApiResponse<AuthResponseDto>>
    {
        public string IdToken { get; } = idToken;
    }
}
