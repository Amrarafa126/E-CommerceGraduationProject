using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record UpdateProfileCommand(
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string? AvatarUrl)
        : IRequest<ApiResponse<UserDto>>;
}
