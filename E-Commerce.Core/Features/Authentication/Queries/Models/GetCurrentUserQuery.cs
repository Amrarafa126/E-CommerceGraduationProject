using MediatR;

namespace E_Commerce.Core.Features.Authentication.Queries.Models
{
    public record GetCurrentUserQuery : IRequest<ApiResponse<UserDto>>;
}
