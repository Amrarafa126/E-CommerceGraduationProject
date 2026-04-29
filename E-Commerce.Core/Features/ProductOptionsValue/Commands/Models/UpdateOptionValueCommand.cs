using E_Commerce.Core.Features.ProductOptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptionsValue.Commands.Models
{
    public record UpdateOptionValueCommand(
      Guid ProductId,
      Guid OptionId,
      Guid ValueId,
      string Value,
      int DisplayOrder = 0)
      : IRequest<ApiResponse<ProductOptionValueDto>>;
}
