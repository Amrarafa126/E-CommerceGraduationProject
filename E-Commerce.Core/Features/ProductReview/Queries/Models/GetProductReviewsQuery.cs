using E_Commerce.Core.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Queries.Models
{
    public record GetProductReviewsQuery(Guid ProductId, int Page = 1, int PageSize = 10)
     : IRequest<ApiResponse<PaginatedResult<ProductReviewDto>>>;
}
