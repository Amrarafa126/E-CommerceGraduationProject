using MediatR;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public record CreateProductCommand(
        string Name, string Description, Guid CategoryId,
        int MinimumOrderQuantity, decimal BasePrice,
        string Currency = "EGP")
        : IRequest<ApiResponse<ProductDto>>;


}

