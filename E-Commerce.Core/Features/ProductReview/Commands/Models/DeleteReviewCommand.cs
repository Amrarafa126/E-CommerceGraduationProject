using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Commands.Models
{
    public record DeleteReviewCommand(Guid ReviewId) : IRequest<ApiResponse<object>>;

}
