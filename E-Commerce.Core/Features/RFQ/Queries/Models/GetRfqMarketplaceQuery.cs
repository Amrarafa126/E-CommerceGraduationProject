using E_Commerce.Core.Wrappers;
using MediatR;

namespace E_Commerce.Core.Features.RFQ.Queries.Models
{
    public record GetRfqMarketplaceQuery(
        int Page = 1,
        int PageSize = 20,
        string? Search = null,
        Guid? CategoryId = null,
        string? Country = null,
        int? Status = null)
        : IRequest<ApiResponse<PaginatedResult<RfqRequestDto>>>;
}
