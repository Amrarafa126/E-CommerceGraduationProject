using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Queries.Models
{
    public record GetReviewStatsQuery(Guid ProductId) : IRequest<ApiResponse<ReviewStatsDto>>;

}
