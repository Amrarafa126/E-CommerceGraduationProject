using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public record PublishProductCommand(Guid ProductId) : IRequest<ApiResponse<ProductDto>>;

}
