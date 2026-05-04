using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Products.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.Products.Queries.Handlers
{
    public class ProductHandlersQueries(IUnitOfWork uow , IMapper mapper , ICurrentUserService currentUser)
    : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>,
      IRequestHandler<GetProductsQuery, ApiResponse<PaginatedResult<ProductSummaryDto>>>,
      IRequestHandler<GetMyProductsQuery, ApiResponse<PaginatedResult<ProductSummaryDto>>>
    {
        public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery req, CancellationToken ct)
        {
            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);
            return ApiResponse<ProductDto>.Ok(ProductMapper.Map(product));
        }

 
            public async Task<ApiResponse<PaginatedResult<ProductSummaryDto>>> Handle(
            GetProductsQuery req, CancellationToken ct)
        {
            var (items, total) = await uow.Products.GetPagedAsync(
                req.Page, req.PageSize, req.CategoryId, req.CompanyId,
                req.Search, req.MinPrice, req.MaxPrice, ct);

            var dtos = mapper.Map<IEnumerable<ProductSummaryDto>>(items);
            return ApiResponse<PaginatedResult<ProductSummaryDto>>.Ok(
                PaginatedResult<ProductSummaryDto>.Success(dtos, total, req.Page, req.PageSize));
        }

        public async Task<ApiResponse<PaginatedResult<ProductSummaryDto>>> Handle(
       GetMyProductsQuery req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var user = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct)
                ?? throw new NotFoundException(nameof(User), currentUser.UserId.Value);

            if (user.OwnedCompanyId == null) throw new BusinessException("User has no associated company.");

            var (items, total) = await uow.Products.GetPagedAsync(
                req.Page, req.PageSize, companyId: user.OwnedCompanyId, ct: ct);

            var dtos = mapper.Map<IEnumerable<ProductSummaryDto>>(items);
            return ApiResponse<PaginatedResult<ProductSummaryDto>>.Ok(
                PaginatedResult<ProductSummaryDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
 }


