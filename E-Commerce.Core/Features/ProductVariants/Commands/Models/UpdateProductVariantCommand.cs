using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductVariants.Commands.Models
{
    public record UpdateProductVariantCommand(
     Guid ProductId,
     Guid VariantId,
     string SKU,
     decimal Price,
     int StockQuantity,
     bool IsActive,
     List<Guid> OptionValueIds)
     : IRequest<ApiResponse<ProductVariantDto>>;
}
