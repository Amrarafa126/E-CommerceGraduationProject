using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.ProductPriceTiers.Commands.Handlers
{
    public class AddPriceTierHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService cu)
      : IRequestHandler<AddPriceTierCommand, ApiResponse<PriceTierDto>>,
         IRequestHandler<UpdatePriceTierCommand, ApiResponse<PriceTierDto>>,
          IRequestHandler<DeletePriceTierCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<PriceTierDto>> Handle(AddPriceTierCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك إضافة مستويات أسعار منتجاتك فقط.");

            bool hasOverlap = product.PriceTiers
                .Any(t =>
                    req.MinQuantity <= (t.MaxQuantity ?? int.MaxValue) &&
                    (req.MaxQuantity ?? int.MaxValue) >= t.MinQuantity);

            if (hasOverlap)
                return ApiResponse<PriceTierDto>.Fail(
                    "The quantity range overlaps with an existing price tier.", 409);

            var tier = ProductPriceTier.Create(product.Id, req.MinQuantity, req.UnitPrice, req.MaxQuantity);

            await uow.PriceTiers.AddAsync(tier, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<PriceTierDto>.Created(mapper.Map<PriceTierDto>(tier));
        }

        public async Task<ApiResponse<PriceTierDto>> Handle(UpdatePriceTierCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل مستويات أسعار منتجاتك فقط.");

            var tier = product.PriceTiers!.FirstOrDefault(t => t.Id == req.TierId)
                ?? throw new NotFoundException(nameof(ProductPriceTier), req.TierId);

            bool hasOverlap = product.PriceTiers
                .Where(t => t.Id != req.TierId)
                .Any(t =>
                    req.MinQuantity <= (t.MaxQuantity ?? int.MaxValue) &&
                    (req.MaxQuantity ?? int.MaxValue) >= t.MinQuantity);

            if (hasOverlap)
                return ApiResponse<PriceTierDto>.Fail(
                    "The quantity range overlaps with an existing price tier.", 409);

            tier.Update(req.MinQuantity, req.UnitPrice, req.MaxQuantity);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<PriceTierDto>.Ok(mapper.Map<PriceTierDto>(tier));
        }
        public async Task<ApiResponse<object>> Handle(DeletePriceTierCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك حذف مستويات أسعار منتجاتك فقط.");

            var tier = product.PriceTiers!.FirstOrDefault(t => t.Id == req.TierId)
                ?? throw new NotFoundException(nameof(ProductPriceTier), req.TierId);

            product.PriceTiers.Remove(tier);

            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("تم حذف مستوى السعر بنجاح.");
        }
    }
}
