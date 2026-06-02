using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Commands.Models
{
    public record CreateReviewCommand(Guid ProductId, int Rating, string Title, string Comment, List<string>? ImageUrls = null)
        : IRequest<ApiResponse<ProductReviewDto>>;
}
