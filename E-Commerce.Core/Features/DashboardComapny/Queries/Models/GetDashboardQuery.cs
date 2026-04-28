using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.DashboardComapny.Queries.Models
{
    public record GetDashboardQuery : IRequest<ApiResponse<CompanyDashboardDto>>;

}
