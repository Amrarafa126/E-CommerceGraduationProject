using E_Commerce.Core.Wrappers;
using MediatR;

namespace E_Commerce.Core.Features.ProductReview.Queries.Models
{
    public record GetMyReviewsQuery(int Page, int PageSize)
        : IRequest<ApiResponse<PaginatedResult<ProductReviewDto>>>;
}
