using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products.Queries.Models
{
    public record GetProductByIdQuery(Guid ProductId) : IRequest<ApiResponse<ProductDto>>;

}
