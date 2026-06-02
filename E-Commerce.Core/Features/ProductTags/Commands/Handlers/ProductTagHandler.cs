using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductTags.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.ProductTags.Commands.Handlers
{
    public class ProductTagHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<AddProductTagCommand, ApiResponse<ProductTagDto>>,
          IRequestHandler<DeleteProductTagCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ProductTagDto>> Handle(AddProductTagCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var existing = product.Tags.FirstOrDefault(t => t.Tag == req.Tag.Trim().ToLowerInvariant() && !t.IsDeleted);
            if (existing != null)
                throw new ConflictException("Tag already exists on this product.");

            var tag = ProductTag.Create(req.ProductId, req.Tag);
            await uow.Tags.AddAsync(tag, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductTagDto>.Created(new ProductTagDto(tag.Id, tag.Tag));
        }

        public async Task<ApiResponse<object>> Handle(DeleteProductTagCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var tag = await uow.Tags.GetByIdAsync(req.TagId, ct)
                ?? throw new NotFoundException("ProductTag", req.TagId);

            if (tag.ProductId != req.ProductId)
                throw new BusinessException("Tag does not belong to this product.");

            tag.SoftDelete();
            uow.Tags.Update(tag);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Tag deleted.");
        }
    }
}
