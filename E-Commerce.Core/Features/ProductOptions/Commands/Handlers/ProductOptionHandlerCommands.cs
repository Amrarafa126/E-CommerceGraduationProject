using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;


namespace E_Commerce.Core.Features.ProductOptions.Commands.Handlers
{
   
        public class ProductOptionHandlerCommands(IUnitOfWork uow, IMapper mapper, ICurrentUserService cu)
     : IRequestHandler<AddProductOptionCommand, ApiResponse<ProductOptionDto>>,
        IRequestHandler<DeleteProductOptionCommand, ApiResponse<object>>,
         IRequestHandler<UpdateProductOptionCommand, ApiResponse<ProductOptionDto>>
    {
            public async Task<ApiResponse<ProductOptionDto>> Handle(AddProductOptionCommand req, CancellationToken ct)
            {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                    ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.Role != "Admin" && cu.OwnedCompanyId != product.CompanyId)
                throw new ForbiddenException("You are not allowed to modify this product.");

            var option = ProductOption.Create(product.Id, req.Name);

                if (req.Values != null)
                {
                    foreach (var val in req.Values)
                        option.Values.Add(ProductOptionValue.Create(option.Id, val)); // خلي بالك انك لازم تضيف ال option قبل ما تضيف ال values عشان تاخد ال optionId اللي هو auto increment
            }

                product.ProductOptions.Add(option);
                uow.Products.Update(product);
                await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionDto>.Created(mapper.Map<ProductOptionDto>(option));
        }

        public async Task<ApiResponse<object>> Handle(
           DeleteProductOptionCommand req, CancellationToken ct)
        {
            // ── 1. Auth ───────────────────────────────────────────────
            if (cu.UserId == null)
                throw new UnauthorizedException();

            // ── 2. Load product (with options + variants) ─────────────
            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            // ── 3. Ownership check ────────────────────────────────────
            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only delete options of your own products.");

            // ── 4. Find the option ────────────────────────────────────
            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            // ── 5. Safety: warn if variants reference this option's values ──
            var optionValueIds = option.Values.Select(v => v.Id).ToHashSet();

            bool variantsUseThisOption = product.productVariants
                .Any(v => v.OptionValues
                    .Any(ov => optionValueIds.Contains(ov.ProductOptionValueId)));

            if (variantsUseThisOption)
                return ApiResponse<object>.Fail(
                    "Cannot delete this option because one or more variants are linked to its values. " +
                    "Delete the related variants first.", 409);

            // ── 6. Remove the option (cascade removes Values via EF config) ──
            product.ProductOptions.Remove(option);
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Option and all its values deleted successfully.");
        }

        public async Task<ApiResponse<ProductOptionDto>> Handle(UpdateProductOptionCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You are not allowed to modify this product.");

            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            option.Update(req.Name, req.DisplayOrder);

            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionDto>.Ok(mapper.Map<ProductOptionDto>(option));
        }
    }
 }

