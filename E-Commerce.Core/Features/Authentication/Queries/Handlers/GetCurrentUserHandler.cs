using AutoMapper;
using E_Commerce.Core.Features.Authentication.Queries.Models;
using E_Commerce.Data.Identity;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Core.Features.Authentication.Queries.Handlers
{
    public class GetCurrentUserHandler(
        UserManager<User> userManager,
        ICurrentUserService currentUser,
        IMapper mapper) : IRequestHandler<GetCurrentUserQuery, ApiResponse<UserDto>>
    {
        public async Task<ApiResponse<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId == null)
                return ApiResponse<UserDto>.Unauthorized("غير مصرح. يرجى تسجيل الدخول.");

            var user = await userManager.FindByIdAsync(currentUser.UserId.Value.ToString());
            if (user == null)
                return ApiResponse<UserDto>.NotFound("المستخدم غير موجود.");

            return ApiResponse<UserDto>.Ok(mapper.Map<UserDto>(user));
        }
    }
}
