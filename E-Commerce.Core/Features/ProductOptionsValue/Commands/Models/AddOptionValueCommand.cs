using E_Commerce.Core.Features.ProductOptions;
using MediatR;

namespace E_Commerce.Core.Features.ProductOptionsValue.Commands.Models
{
    public record AddOptionValueCommand(
        Guid ProductId,
        Guid OptionId,
        string Value,
        int DisplayOrder = 0)
        : IRequest<ApiResponse<ProductOptionValueDto>>;
}
