

using E_Commerce.Core.BaseResponse;
using E_Commerce.Data.Result;
using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record LoginCommand(string Email, string Password)
     : IRequest<ApiResponse<AuthResponseDto>>;
}
