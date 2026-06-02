using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Commands.Models;
using E_Commerce.Data.Identity;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Commands.Handlers
{

    public class AdminManageUserHandler(
        UserManager<User> userManager,
        ICurrentUserService cu)
        : IRequestHandler<AdminManageUserCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<object>> Handle(AdminManageUserCommand req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("مسموح فقط للمسؤول.");

            var user = await userManager.FindByIdAsync(req.UserId.ToString())
                ?? throw new NotFoundException(nameof(User), req.UserId);

            switch (req.Action)
            {
                case AdminUserAction.Activate:
                    user.Activate();
                    break;
                case AdminUserAction.Deactivate:
                    user.Deactivate();
                    break;
                case AdminUserAction.Lock:
                    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1));
                    break;
                case AdminUserAction.Unlock:
                    await userManager.SetLockoutEndDateAsync(user, null);
                    await userManager.ResetAccessFailedCountAsync(user);
                    break;
                case AdminUserAction.VerifyEmail:
                    user.VerifyEmail();
                    break;
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BusinessException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return ApiResponse<object>.Ok($"تم تطبيق إجراء {req.Action} على المستخدم بنجاح.");
        }
    }

}
