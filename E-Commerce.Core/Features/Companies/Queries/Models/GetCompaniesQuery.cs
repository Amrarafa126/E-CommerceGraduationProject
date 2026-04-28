using E_Commerce.Core.Wrappers;
using MediatR;


namespace E_Commerce.Core.Features.Companies.Queries.Models
{
    public record GetCompaniesQuery(string? Search, int Page = 1, int PageSize = 20)
       : IRequest<ApiResponse<PaginatedResult<CompanySummaryDto>>>;
}
