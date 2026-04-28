using MediatR;

namespace E_Commerce.Core.Features.ProductOptions.Commands.Models
{
    public record AddProductOptionCommand(
     Guid ProductId, string Name, int DisplayOrder = 0,
     List<string>? Values = null)
     : IRequest<ApiResponse<ProductOptionDto>>;
}
