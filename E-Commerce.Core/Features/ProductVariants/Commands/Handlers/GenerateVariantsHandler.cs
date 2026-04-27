using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce.Core.Features.ProductVariants.Commands.Handlers
{
    public class AddProductVariantHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<AddProductVariantCommand, ApiResponse<ProductVariantDto>>
    {
        public async Task<ApiResponse<ProductVariantDto>> Handle(AddProductVariantCommand req, CancellationToken ct)
        {
            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            // Check SKU uniqueness within product
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
    }
}
