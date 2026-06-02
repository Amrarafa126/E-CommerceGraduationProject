using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public record UpdateProductCommand(
    Guid ProductId, string Name, string Description, Guid CategoryId,
    int MinimumOrderQuantity, decimal BasePrice, int StockQuantity)
    : IRequest<ApiResponse<ProductDto>>;
}
