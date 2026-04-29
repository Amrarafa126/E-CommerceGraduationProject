using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptionsValue.Commands.Models
{
    public record DeleteOptionValueCommand(
         Guid ProductId,
         Guid OptionId,
         Guid ValueId)
         : IRequest<ApiResponse<object>>;
}
