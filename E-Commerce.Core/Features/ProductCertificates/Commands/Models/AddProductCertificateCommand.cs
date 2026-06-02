using E_Commerce.Core.Features.Products;
using MediatR;

namespace E_Commerce.Core.Features.ProductCertificates.Commands.Models
{
    public record AddProductCertificateCommand(
        Guid ProductId, string Name, string Url,
        string OriginalFileName, string ContentType, long FileSizeBytes,
        string? IssuedBy, DateTime? ValidUntil)
        : IRequest<ApiResponse<ProductCertificateDto>>;

    public record DeleteProductCertificateCommand(
        Guid ProductId, Guid CertificateId)
        : IRequest<ApiResponse<object>>;
}
