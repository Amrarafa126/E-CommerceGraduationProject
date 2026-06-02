using E_Commerce.Core.Features.Products;
using MediatR;

namespace E_Commerce.Core.Features.ProductTags.Commands.Models
{
    public record AddProductTagCommand(Guid ProductId, string Tag)
        : IRequest<ApiResponse<ProductTagDto>>;

    public record DeleteProductTagCommand(Guid ProductId, Guid TagId)
        : IRequest<ApiResponse<object>>;
}
