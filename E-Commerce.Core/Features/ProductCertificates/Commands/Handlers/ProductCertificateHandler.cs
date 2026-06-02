using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductCertificates.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.ProductCertificates.Commands.Handlers
{
    public class ProductCertificateHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<AddProductCertificateCommand, ApiResponse<ProductCertificateDto>>,
          IRequestHandler<DeleteProductCertificateCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ProductCertificateDto>> Handle(AddProductCertificateCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var cert = ProductCertificate.Create(req.ProductId, req.Name, req.Url, req.OriginalFileName,
                req.ContentType, req.FileSizeBytes, req.IssuedBy, req.ValidUntil);

            await uow.Certificates.AddAsync(cert, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductCertificateDto>.Created(new ProductCertificateDto(
                cert.Id, cert.Name, cert.Url, cert.OriginalFileName, cert.ContentType, cert.FileSizeBytes,
                cert.IssuedBy, cert.ValidUntil, cert.DisplayOrder));
        }

        public async Task<ApiResponse<object>> Handle(DeleteProductCertificateCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var cert = await uow.Certificates.GetByIdAsync(req.CertificateId, ct)
                ?? throw new NotFoundException("ProductCertificate", req.CertificateId);

            if (cert.ProductId != req.ProductId)
                throw new BusinessException("الشهادة لا تتبع هذا المنتج.");

            cert.SoftDelete();
            uow.Certificates.Update(cert);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("تم حذف الشهادة.");
        }
    }
}
