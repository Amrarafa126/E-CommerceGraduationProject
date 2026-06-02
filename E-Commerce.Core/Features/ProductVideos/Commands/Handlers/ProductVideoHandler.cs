using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductVideos.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.ProductVideos.Commands.Handlers
{
    public class ProductVideoHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<AddProductVideoCommand, ApiResponse<ProductVideoDto>>,
          IRequestHandler<DeleteProductVideoCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ProductVideoDto>> Handle(AddProductVideoCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var video = ProductVideo.Create(req.ProductId, req.Url, req.Title, req.ThumbnailUrl, 0, req.DurationSeconds);

            await uow.Videos.AddAsync(video, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductVideoDto>.Created(new ProductVideoDto(
                video.Id, video.Url, video.Title, video.ThumbnailUrl, video.DisplayOrder, video.DurationSeconds));
        }

        public async Task<ApiResponse<object>> Handle(DeleteProductVideoCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            var video = await uow.Videos.GetByIdAsync(req.VideoId, ct)
                ?? throw new NotFoundException("ProductVideo", req.VideoId);

            if (video.ProductId != req.ProductId)
                throw new BusinessException("Video does not belong to this product.");

            video.SoftDelete();
            uow.Videos.Update(video);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Video deleted.");
        }
    }
}
