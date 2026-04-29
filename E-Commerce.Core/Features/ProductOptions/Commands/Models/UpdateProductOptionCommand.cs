using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptions.Commands.Models
{
    public record UpdateProductOptionCommand(
     Guid ProductId,
     Guid OptionId,
     string Name,
     int DisplayOrder = 0)
     : IRequest<ApiResponse<ProductOptionDto>>;
}
