using E_Commerce.Core.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Models
{
    public record GetAllUsersQuery(
      string? Search = null, string? Role = null,
      bool? IsActive = null, int Page = 1, int PageSize = 20)
      : IRequest<ApiResponse<PaginatedResult<AdminUserDto>>>;
}
