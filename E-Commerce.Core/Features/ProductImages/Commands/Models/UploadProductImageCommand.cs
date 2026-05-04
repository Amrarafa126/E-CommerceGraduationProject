using MediatR;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.Features.ProductImages.Commands.Models
{
    public record UploadProductImageCommand(
        Guid ProductId, IFormFile File,
        string? AltText = null, int DisplayOrder = 0)
        : IRequest<ApiResponse<ProductImageDto>>;
}
