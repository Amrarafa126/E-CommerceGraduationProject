using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Commands.Models
{
    public record ReplyToReviewCommand(Guid ReviewId, string Reply)
     : IRequest<ApiResponse<ProductReviewDto>>;
}
