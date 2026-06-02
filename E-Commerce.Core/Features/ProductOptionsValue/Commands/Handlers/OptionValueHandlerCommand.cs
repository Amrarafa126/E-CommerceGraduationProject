using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductOptionsValue.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptionsValue.Commands.Handlers
{
    public class OptionValueHandlerCommand(IUnitOfWork uow, IMapper mapper, ICurrentUserService cu):
          IRequestHandler<AddOptionValueCommand, ApiResponse<ProductOptionValueDto>>,
          IRequestHandler<UpdateOptionValueCommand, ApiResponse<ProductOptionValueDto>>,
          IRequestHandler<DeleteOptionValueCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ProductOptionValueDto>> Handle(AddOptionValueCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("غير مسموح لك بتعديل هذا المنتج.");

            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            // Check duplicate
            if (option.Values.Any(v => v.Value.Equals(req.Value, StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<ProductOptionValueDto>.Fail(
                    $"Value '{req.Value}' already exists in this option.", 409);

            var value = ProductOptionValue.Create(req.OptionId, req.Value);
            value.DisplayOrder = req.DisplayOrder;
            option.Values.Add(value);

            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionValueDto>.Ok(
                new ProductOptionValueDto(value.Id, value.Value, value.DisplayOrder));
        }

        public async Task<ApiResponse<ProductOptionValueDto>> Handle(UpdateOptionValueCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("غير مسموح لك بتعديل هذا المنتج.");

            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            var value = option.Values.FirstOrDefault(v => v.Id == req.ValueId)
                ?? throw new NotFoundException(nameof(ProductOptionValue), req.ValueId);

            // Check duplicate (excluding self)
            if (option.Values.Any(v => v.Id != req.ValueId &&
                v.Value.Equals(req.Value, StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<ProductOptionValueDto>.Fail(
                    $"Value '{req.Value}' already exists in this option.", 409);

            value.Update(req.Value, req.DisplayOrder);

            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionValueDto>.Ok(
                new ProductOptionValueDto(value.Id, value.Value, value.DisplayOrder));
        }

        // ─────────────────────────────────────────────────────────────────
        // DELETE VALUE
        // ─────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<object>> Handle(DeleteOptionValueCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("غير مسموح لك بتعديل هذا المنتج.");

            var option = product.ProductOptions.FirstOrDefault(o => o.Id == req.OptionId)
                ?? throw new NotFoundException(nameof(ProductOption), req.OptionId);

            var value = option.Values.FirstOrDefault(v => v.Id == req.ValueId)
                ?? throw new NotFoundException(nameof(ProductOptionValue), req.ValueId);

            // Safety: block delete if any variant uses this value
            bool variantsUseValue = product.productVariants
                .Any(v => v.OptionValues.Any(ov => ov.ProductOptionValueId == req.ValueId));

            if (variantsUseValue)
                return ApiResponse<object>.Fail(
                    "Cannot delete this value because one or more variants are linked to it. " +
                    "احذف المتغيرات المرتبطة أولاً.", 409);

            option.Values.Remove(value);
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("تم حذف قيمة الخيار بنجاح.");
        }
    }
}
