using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Status;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Models
{
    public record GetAllCompaniesQuery(
     string? Search = null, CompanyStatus? Status = null,
     int Page = 1, int PageSize = 20)
     : IRequest<ApiResponse<PaginatedResult<AdminCompanyDto>>>;
}
