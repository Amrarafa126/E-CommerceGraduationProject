using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductReview.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Queries.Handlers
{
    public class ProductReviewHandlersQueries(IUnitOfWork uow)
    : IRequestHandler<GetProductReviewsQuery, ApiResponse<PaginatedResult<ProductReviewDto>>>,
        IRequestHandler<GetReviewStatsQuery, ApiResponse<ReviewStatsDto>>
    
    {
        public async Task<ApiResponse<PaginatedResult<ProductReviewDto>>> Handle(
        GetProductReviewsQuery req, CancellationToken ct)
        {
            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            var (items, total) = await uow.Reviews.GetByProductAsync(req.ProductId, req.Page, req.PageSize, ct);

            var dtos = items.Select(r => new ProductReviewDto(
                r.Id, r.ProductId, product.Name,
                r.BuyerId, r.Buyer?.FullName ?? "",
                r.Rating, r.Title, r.Comment, r.IsVerifiedPurchase,
                r.SupplierReply, r.RepliedAt,
                r.Images.Select(i => i.Url).ToList(),
                r.CreatedAt, r.UpdatedAt)).ToList();

            return ApiResponse<PaginatedResult<ProductReviewDto>>.Ok(
                PaginatedResult<ProductReviewDto>.Success(dtos, total, req.Page, req.PageSize));
        }

        public async Task<ApiResponse<ReviewStatsDto>> Handle(GetReviewStatsQuery req, CancellationToken ct)
        {
            _ = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            var (avg, count, dist) = await uow.Reviews.GetStatsAsync(req.ProductId, ct);
            return ApiResponse<ReviewStatsDto>.Ok(new ReviewStatsDto(avg, count, dist));
        }
    }
}
