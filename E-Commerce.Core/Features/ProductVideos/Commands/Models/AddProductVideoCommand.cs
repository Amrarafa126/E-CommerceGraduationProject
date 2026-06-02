using E_Commerce.Core.Features.Products;
using MediatR;

namespace E_Commerce.Core.Features.ProductVideos.Commands.Models
{
    public record AddProductVideoCommand(
        Guid ProductId, string Url, string? Title,
        string? ThumbnailUrl, int? DurationSeconds)
        : IRequest<ApiResponse<ProductVideoDto>>;

    public record DeleteProductVideoCommand(
        Guid ProductId, Guid VideoId)
        : IRequest<ApiResponse<object>>;
}
