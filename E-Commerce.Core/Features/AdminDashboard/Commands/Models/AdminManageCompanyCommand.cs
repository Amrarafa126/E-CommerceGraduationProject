using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Commands.Models
{
    public record AdminManageCompanyCommand(
      Guid CompanyId, CompanyAdminAction Action)
      : IRequest<ApiResponse<object>>;

    public enum CompanyAdminAction { Approve, Reject, Suspend, Reactivate }

}
