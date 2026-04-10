using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce.Core.Features.ProductVariants.Commands.Handlers
{
    public class GenerateVariantsHandler : ResponseHandler, IRequestHandler<GenerateVariantsCommand, Response<string>>
    {
        private readonly AppDBContext _context;
        private readonly IVariantService _variantService;

        public GenerateVariantsHandler(
            AppDBContext context,
            IVariantService variantService)
        {
            _context = context;
            _variantService = variantService;
        }

        public async Task<Response<string>> Handle(GenerateVariantsCommand request,CancellationToken cancellationToken)
        {
            var product = await _context.products
                .Include(p => p.ProductOptions)
                    .ThenInclude(o => o.Values)
                .Include(p => p.productVariants)
                    .ThenInclude(v => v.VariantValues)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null)
                return  NotFound<string>("Product not found");


            var variants = _variantService.BuildVariants(product);

            if (!variants.Any())
                return Success<string>("No new variants generated");

            await _context.productVariants.AddRangeAsync(variants);
            await _context.SaveChangesAsync();

            return Success<string>("Variants Generated Successfully");
        }
    }
}
