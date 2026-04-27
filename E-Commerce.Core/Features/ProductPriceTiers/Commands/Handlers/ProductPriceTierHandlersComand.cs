using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;


namespace E_Commerce.Core.Features.ProductPriceTiers.Commands.Handlers
{
    public class AddPriceTierHandler(IUnitOfWork uow, IMapper mapper)
      : IRequestHandler<AddPriceTierCommand, ApiResponse<PriceTierDto>>
    {
        public async Task<ApiResponse<PriceTierDto>> Handle(AddPriceTierCommand req, CancellationToken ct)
        {
            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            var tier = ProductPriceTier.Create(product.Id, req.MinQuantity, req.UnitPrice, req.MaxQuantity);
            product.PriceTiers.Add(tier);

            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<PriceTierDto>.Created(mapper.Map<PriceTierDto>(tier));
        }
    }


}
