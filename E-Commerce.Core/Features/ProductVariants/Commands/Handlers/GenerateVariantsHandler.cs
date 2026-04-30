using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.ProductVariants.Commands.Handlers
{
    public class AddProductVariantHandler(IUnitOfWork uow, IMapper mapper,ICurrentUserService cu)
    : IRequestHandler<AddProductVariantCommand, ApiResponse<ProductVariantDto>>,
         IRequestHandler<UpdateProductVariantCommand, ApiResponse<ProductVariantDto>>,
          IRequestHandler<DeleteProductVariantCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ProductVariantDto>> Handle(AddProductVariantCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only Add variants of your own products.");

            if (product.productVariants.Any(v => v.SKU == req.SKU))
                throw new ConflictException($"SKU '{req.SKU}' already exists in this product.");

            var variant = ProductVariant.Create(product.Id, req.SKU, req.Price, req.StockQuantity);

            foreach (var optionValueId in req.OptionValueIds)
            {
                variant.OptionValues.Add(ProductVariantOptionValue.Create(variant.Id, optionValueId));
            }

            product.productVariants.Add(variant);
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductVariantDto>.Created(mapper.Map<ProductVariantDto>(variant));
        }

        public async Task<ApiResponse<ProductVariantDto>> Handle(UpdateProductVariantCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only edit variants of your own products.");

            var variant = product.productVariants.FirstOrDefault(v => v.Id == req.VariantId)
                ?? throw new NotFoundException(nameof(ProductVariant), req.VariantId);

            if (product.productVariants.Any(v => v.Id != req.VariantId && v.SKU == req.SKU))
                return ApiResponse<ProductVariantDto>.Fail(
                    $"SKU '{req.SKU}' already exists in another variant of this product.", 409);

            variant.UpdateSku(req.SKU); 
            variant.UpdatePrice(req.Price);
            variant.UpdateStock(req.StockQuantity);

            if (req.IsActive) variant.Activate();
            else variant.Deactivate();

            variant.OptionValues.Clear();
            foreach (var optionValueId in req.OptionValueIds)
                variant.OptionValues.Add(ProductVariantOptionValue.Create(variant.Id, optionValueId));

            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductVariantDto>.Ok(mapper.Map<ProductVariantDto>(variant));
        }

       
        public async Task<ApiResponse<object>> Handle(DeleteProductVariantCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only delete variants of your own products.");

            var variant = product.productVariants.FirstOrDefault(v => v.Id == req.VariantId)
                ?? throw new NotFoundException(nameof(ProductVariant), req.VariantId);

            product.productVariants.Remove(variant);
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Variant deleted successfully.");
        }
    }
}
