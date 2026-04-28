using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record RegisterBuyerCommand(
     string Email, string Password,
     string FirstName, string LastName, string? PhoneNumber)
     : IRequest<ApiResponse<AuthResponseDto>>;
}
