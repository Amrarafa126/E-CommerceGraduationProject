using E_Commerce.Core.Wrappers;
using MediatR;

namespace E_Commerce.Core.Features.ProductReview.Queries.Models
{
    public record GetSellerReviewsQuery(int Page = 1, int PageSize = 10)
        : IRequest<ApiResponse<PaginatedResult<ProductReviewDto>>>;
}
