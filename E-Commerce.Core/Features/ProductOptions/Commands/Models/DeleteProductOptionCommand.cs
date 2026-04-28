using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptions.Commands.Models
{
    public record DeleteProductOptionCommand(Guid ProductId,Guid OptionId)
       : IRequest<ApiResponse<object>>;
}
