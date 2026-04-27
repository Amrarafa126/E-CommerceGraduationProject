

using E_Commerce.Core.BaseResponse;
using E_Commerce.Data.Result;
using MediatR;

namespace E_Commerce.Core.Features.Authentication.Commands.Models
{
    public class SignInCommand : IRequest<Response<JwtAuthResult>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
