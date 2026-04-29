using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductVariants.Commands.Models
{
    public record DeleteProductVariantCommand(
         Guid ProductId,
         Guid VariantId)
         : IRequest<ApiResponse<object>>;
}
