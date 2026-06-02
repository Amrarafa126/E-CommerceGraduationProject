using E_Commerce.Core.Features.Products;
using MediatR;

namespace E_Commerce.Core.Features.ProductSpecifications.Commands.Models
{
    public record AddProductSpecificationCommand(
        Guid ProductId, string Name, string Value,
        string? GroupName, int DisplayOrder, bool IsHighlight)
        : IRequest<ApiResponse<ProductSpecificationDto>>;

    public record UpdateProductSpecificationCommand(
        Guid ProductId, Guid SpecificationId, string Name, string Value,
        string? GroupName, int DisplayOrder, bool IsHighlight)
        : IRequest<ApiResponse<ProductSpecificationDto>>;

    public record DeleteProductSpecificationCommand(
        Guid ProductId, Guid SpecificationId)
        : IRequest<ApiResponse<object>>;
}
