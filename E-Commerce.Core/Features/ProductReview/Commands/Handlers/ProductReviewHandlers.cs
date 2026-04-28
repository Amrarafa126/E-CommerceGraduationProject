using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductReview.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Commands.Handlers
{
    public class ProductReviewHandlers(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<CreateReviewCommand, ApiResponse<ProductReviewDto>>,
      IRequestHandler<UpdateReviewCommand, ApiResponse<ProductReviewDto>>,
      IRequestHandler<DeleteReviewCommand, ApiResponse<object>>,
      IRequestHandler<ReplyToReviewCommand, ApiResponse<ProductReviewDto>>
    {
        public async Task<ApiResponse<ProductReviewDto>> Handle(CreateReviewCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            var alreadyReviewed = await uow.Reviews.HasBuyerReviewedAsync(
                req.ProductId, currentUser.UserId.Value, ct);

            if (alreadyReviewed)
                throw new ConflictException("You have already reviewed this product.");

            // Check if buyer has purchased this product (verified purchase)
            var hasPurchased = await uow.Orders.ExistsAsync(o =>
                o.BuyerId == currentUser.UserId.Value &&
                o.Items.Any(i => i.ProductId == req.ProductId), ct);

            var review = Data.Entity.ProductReview.Create(
                req.ProductId, currentUser.UserId.Value,
                req.Rating, req.Title, req.Comment, hasPurchased);

            await uow.Reviews.AddAsync(review, ct);
            await uow.SaveChangesAsync(ct);

            var buyer = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct);
            return ApiResponse<ProductReviewDto>.Created(MapReview(review, buyer!, product));
        }



    private static ProductReviewDto MapReview(Data.Entity.ProductReview r, User buyer, Product product) =>
        new(r.Id, r.ProductId, product.Name, r.BuyerId, buyer.FullName,
            r.Rating, r.Title, r.Comment, r.IsVerifiedPurchase,
            r.SupplierReply, r.RepliedAt,
            r.Images.Select(i => i.Url).ToList(),
            r.CreatedAt, r.UpdatedAt);

        public async Task<ApiResponse<ProductReviewDto>> Handle(UpdateReviewCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var review = await uow.Reviews.GetWithImagesAsync(req.ReviewId, ct)
                ?? throw new NotFoundException(nameof(ProductReview), req.ReviewId);

            if (review.BuyerId != currentUser.UserId.Value)
                throw new ForbiddenException("You can only update your own reviews.");

            review.Update(req.Rating, req.Title, req.Comment);
            uow.Reviews.Update(review);
            await uow.SaveChangesAsync(ct);

            var buyer = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct);
            var product = await uow.Products.GetByIdAsync(review.ProductId, ct);
            return ApiResponse<ProductReviewDto>.Ok(new ProductReviewDto(
                review.Id, review.ProductId, product!.Name,
                review.BuyerId, buyer!.FullName,
                review.Rating, review.Title, review.Comment,
                review.IsVerifiedPurchase, review.SupplierReply, review.RepliedAt,
                review.Images.Select(i => i.Url).ToList(),
                review.CreatedAt, review.UpdatedAt));
        }

        public async Task<ApiResponse<object>> Handle(DeleteReviewCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var review = await uow.Reviews.GetByIdAsync(req.ReviewId, ct)
                ?? throw new NotFoundException(nameof(ProductReview), req.ReviewId);

            bool isOwner = review.BuyerId == currentUser.UserId.Value;
            bool isAdmin = currentUser.Role == "Admin";

            if (!isOwner && !isAdmin)
                throw new ForbiddenException("You cannot delete this review.");

            review.SoftDelete();
            uow.Reviews.Update(review);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Review deleted successfully.");
        }

        public async Task<ApiResponse<ProductReviewDto>> Handle(ReplyToReviewCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var review = await uow.Reviews.GetWithImagesAsync(req.ReviewId, ct)
                ?? throw new NotFoundException(nameof(ProductReview), req.ReviewId);

            var product = await uow.Products.GetByIdAsync(review.ProductId, ct)!;
            var user = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct)!;

            // Must be a supplier who owns this product's company
            if (user?.OwnedCompanyId != product?.CompanyId && currentUser.Role != "Admin")
                throw new ForbiddenException("Only the product's supplier can reply to reviews.");

            review.AddSupplierReply(req.Reply);
            uow.Reviews.Update(review);
            await uow.SaveChangesAsync(ct);

            var buyer = await uow.Users.GetByIdAsync(review.BuyerId, ct);
            return ApiResponse<ProductReviewDto>.Ok(new ProductReviewDto(
                review.Id, review.ProductId, product!.Name,
                review.BuyerId, buyer!.FullName,
                review.Rating, review.Title, review.Comment,
                review.IsVerifiedPurchase, review.SupplierReply, review.RepliedAt,
                review.Images.Select(i => i.Url).ToList(),
                review.CreatedAt, review.UpdatedAt));
        }
    }
}
