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
        :IRequestHandler<AddProductOptionCommand, ApiResponse<ProductOptionDto>>,
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
                throw new ForbiddenException("غير مسموح لك بتعديل هذا المنتج.");

            var option = ProductOption.Create(product.Id, req.Name);
            product.ProductOptions.Add(option);

            if (req.Values != null && req.Values.Count > 0)
            {
                foreach (var val in req.Values)
                    option.Values.Add(ProductOptionValue.Create(option.Id, val));
            }

            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionDto>.Created(mapper.Map<ProductOptionDto>(option));
        }

        public async Task<ApiResponse<object>> Handle(
           DeleteProductOptionCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك حذف خيارات منتجاتك فقط.");

            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            var optionValueIds = option.Values.Select(v => v.Id).ToHashSet();

            bool variantsUseThisOption = product.productVariants
                .Any(v => v.OptionValues
                    .Any(ov => optionValueIds.Contains(ov.ProductOptionValueId)));

            if (variantsUseThisOption)
                return ApiResponse<object>.Fail(
                    "Cannot delete this option because one or more variants are linked to its values. " +
                    "احذف المتغيرات المرتبطة أولاً.", 409);

            product.ProductOptions.Remove(option);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("تم حذف الخيار وجميع قيمه بنجاح.");
        }

        public async Task<ApiResponse<ProductOptionDto>> Handle(UpdateProductOptionCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("غير مسموح لك بتعديل هذا المنتج.");

            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            option.Update(req.Name, req.DisplayOrder);

            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionDto>.Ok(mapper.Map<ProductOptionDto>(option));
        }
    }
 }

