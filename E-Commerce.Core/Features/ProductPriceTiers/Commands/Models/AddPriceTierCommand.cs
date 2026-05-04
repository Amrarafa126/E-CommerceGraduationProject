using MediatR;

namespace E_Commerce.Core.Features.ProductPriceTiers.Commands.Models
{
    public record AddPriceTierCommand(Guid ProductId, int MinQuantity, decimal UnitPrice, int? MaxQuantity = null)
      : IRequest<ApiResponse<PriceTierDto>>;
}
