using E_Commerce.Data.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken)
        : IRequest<ApiResponse<AuthResponseDto>>;
}
