using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Commands.Models
{
    public record AdminManageUserCommand(
    Guid UserId,
    AdminUserAction Action,
    string? Reason = null)
    : IRequest<ApiResponse<object>>;


    public enum AdminUserAction { Activate, Deactivate, Lock, Unlock, VerifyEmail }

}
