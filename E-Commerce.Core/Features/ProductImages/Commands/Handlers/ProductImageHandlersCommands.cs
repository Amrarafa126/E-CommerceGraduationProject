
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.ProductImages.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.Features.ProductImages.Commands.Handlers
{
    public class ProductImageHandlersCommands(IUnitOfWork uow, ICurrentUserService cu, IFileStorageService storage)
    : IRequestHandler<UploadProductImageCommand, ApiResponse<ProductImageDto>>,
         IRequestHandler<DeleteProductImageCommand, ApiResponse<object>>
    {
        private const int MaxImages = 6;

        public async Task<ApiResponse<ProductImageDto>> Handle(
            UploadProductImageCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            // OwnedCompanyId from JWT — no extra DB call needed
            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")//////////////////////////////////////////////////////
                throw new ForbiddenException("You can only upload images to your own products.");

            if (product.ActiveImageCount >= MaxImages)
                throw new BusinessException($"A product can have at most {MaxImages} images.");

            await using var stream = req.File.OpenReadStream();
            var url = await storage.UploadAsync(
                stream, req.File.FileName, $"products/{product.Id}", ct);

            var image = ProductImage.Create(
                product.Id, url, req.File.FileName,
                req.File.ContentType, req.File.Length,
                req.DisplayOrder, req.AltText);

            product.AddImage(image);
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductImageDto>.Created(new ProductImageDto(
                image.Id, image.Url, image.OriginalFileName,
                image.FileSizeBytes, image.AltText, image.DisplayOrder));
        
        }

        public async Task<ApiResponse<object>> Handle(
        DeleteProductImageCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only delete images from your own products.");

            var image = product.Images.FirstOrDefault(i => i.Id == req.ImageId)
                ?? throw new NotFoundException(nameof(ProductImage), req.ImageId);

            await storage.DeleteAsync(image.Url, ct);
            product.RemoveImage(req.ImageId);
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Image deleted successfully.");
        }
    }

}


