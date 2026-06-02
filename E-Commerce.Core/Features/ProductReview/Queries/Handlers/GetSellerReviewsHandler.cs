using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductReview.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.ProductReview.Queries.Handlers
{
    public class GetSellerReviewsHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<GetSellerReviewsQuery, ApiResponse<PaginatedResult<ProductReviewDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<ProductReviewDto>>> Handle(
            GetSellerReviewsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            if (cu.OwnedCompanyId == null)
                throw new ForbiddenException("لا تمتلك شركة.");

            var (items, total) = await uow.Reviews.GetByCompanyAsync(
                cu.OwnedCompanyId.Value, req.Page, req.PageSize, ct);

            var dtos = items.Select(r => new ProductReviewDto(
                r.Id, r.ProductId, r.Product?.Name ?? "",
                r.BuyerId, r.Buyer?.FullName ?? "",
                r.Rating, r.Title, r.Comment,
                r.IsVerifiedPurchase, r.SupplierReply, r.RepliedAt,
                r.Images.Select(i => i.Url).ToList(),
                r.CreatedAt, r.UpdatedAt)).ToList();

            return ApiResponse<PaginatedResult<ProductReviewDto>>.Ok(
                PaginatedResult<ProductReviewDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
