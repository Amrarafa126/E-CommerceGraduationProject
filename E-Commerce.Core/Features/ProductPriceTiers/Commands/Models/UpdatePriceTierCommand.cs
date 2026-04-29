using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductPriceTiers.Commands.Models
{
    public record UpdatePriceTierCommand(
          Guid ProductId,
          Guid TierId,
          int MinQuantity,
          decimal UnitPrice,
          int? MaxQuantity = null)
          : IRequest<ApiResponse<PriceTierDto>>;
}
