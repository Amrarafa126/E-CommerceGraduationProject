using E_Commerce.Core.Wrappers;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Queries.Models
{
    public record GetAllPayoutsQuery(
        int? Status = null,
        int Page = 1,
        int PageSize = 20)
        : IRequest<ApiResponse<PaginatedResult<PayoutDto>>>;
}
