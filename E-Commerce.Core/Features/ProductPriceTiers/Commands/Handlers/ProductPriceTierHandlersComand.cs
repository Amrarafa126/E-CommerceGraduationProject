using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.ProductPriceTiers.Commands.Handlers
{
    public class ProductPriceTierHandlersComand : ResponseHandler, IRequestHandler<AddPriceTierCommand, Response<string>>
    {
        IProductPriceTierService ProductPriceTierService;
        IMapper mapper;
        AppDBContext context;
        public ProductPriceTierHandlersComand(IProductPriceTierService productPriceTierService , IMapper mapper , AppDBContext appDBContext)
        {
            ProductPriceTierService = productPriceTierService;
            this.mapper = mapper;
            context = appDBContext;

        }

        public async Task<Response<string>> Handle(AddPriceTierCommand request, CancellationToken cancellationToken)
        {
            var tiers = await context.productPriceTiers
                                .Where(x => x.ProductId == request.ProductId)
                                .ToListAsync();

            bool overlap = tiers.Any(t =>
                request.MinQuantity <= t.MaxQuantity &&
                request.MaxQuantity >= t.MinQuantity
            );

            if (overlap)
                throw new Exception("Price tier overlaps with existing tier");

            bool duplicate = tiers.Any(t =>
                               t.MinQuantity == request.MinQuantity &&
                               t.MaxQuantity == request.MaxQuantity);
            if (duplicate)
                throw new Exception("Duplicate price tier");

            if (request.MinQuantity >= request.MaxQuantity)
                return BadRequest<string>("MinQuantity must be less than MaxQuantity");
            var ProductPriceTierMapper = mapper.Map<ProductPriceTier>(request);
            var result = await ProductPriceTierService.AddPriceTierAsync(ProductPriceTierMapper);
            if (result == null)
                return UnprocessableEntity<string>();
            return Success(result);
        }
    }
}
