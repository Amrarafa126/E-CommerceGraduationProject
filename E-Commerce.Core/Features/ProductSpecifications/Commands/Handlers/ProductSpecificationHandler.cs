using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductSpecifications.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.ProductSpecifications.Commands.Handlers
{
    public class ProductSpecificationHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<AddProductSpecificationCommand, ApiResponse<ProductSpecificationDto>>,
          IRequestHandler<UpdateProductSpecificationCommand, ApiResponse<ProductSpecificationDto>>,
          IRequestHandler<DeleteProductSpecificationCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ProductSpecificationDto>> Handle(AddProductSpecificationCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var spec = ProductSpecification.Create(req.ProductId, req.Name, req.Value, req.GroupName, req.DisplayOrder, req.IsHighlight);
            await uow.Specifications.AddAsync(spec, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductSpecificationDto>.Created(new ProductSpecificationDto(
                spec.Id, spec.Name, spec.Value, spec.GroupName, spec.DisplayOrder, spec.IsHighlight));
        }

        public async Task<ApiResponse<ProductSpecificationDto>> Handle(UpdateProductSpecificationCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var spec = await uow.Specifications.GetByIdAsync(req.SpecificationId, ct)
                ?? throw new NotFoundException("ProductSpecification", req.SpecificationId);

            if (spec.ProductId != req.ProductId)
                throw new BusinessException("Specification does not belong to this product.");

            spec.Update(req.Name, req.Value, req.GroupName, req.DisplayOrder, req.IsHighlight);
            uow.Specifications.Update(spec);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductSpecificationDto>.Ok(new ProductSpecificationDto(
                spec.Id, spec.Name, spec.Value, spec.GroupName, spec.DisplayOrder, spec.IsHighlight));
        }

        public async Task<ApiResponse<object>> Handle(DeleteProductSpecificationCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var spec = await uow.Specifications.GetByIdAsync(req.SpecificationId, ct)
                ?? throw new NotFoundException("ProductSpecification", req.SpecificationId);

            if (spec.ProductId != req.ProductId)
                throw new BusinessException("Specification does not belong to this product.");

            spec.SoftDelete();
            uow.Specifications.Update(spec);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Specification deleted.");
        }
    }
}
