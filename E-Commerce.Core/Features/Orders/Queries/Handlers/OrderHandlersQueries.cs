using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
namespace E_Commerce.Core.Features.Orders.Queries.Handlers
{
    public class OrderHandlersQueries(IUnitOfWork uow ,ICurrentUserService cu)
    : IRequestHandler<GetOrderByIdQuery, ApiResponse<OrderDto>>,
      IRequestHandler<GetMyOrdersQuery, ApiResponse<PaginatedResult<OrderDto>>>,
      IRequestHandler<GetSellerOrdersQuery, ApiResponse<PaginatedResult<OrderDto>>>
    {
        public async Task<ApiResponse<OrderDto>> Handle(GetOrderByIdQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && order.SellerCompanyId == cu.OwnedCompanyId.Value;
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("You are not authorized to view this order.");

            return ApiResponse<OrderDto>.Ok(MapExt.MapOrder(order));
        }

        public async Task<ApiResponse<PaginatedResult<OrderDto>>> Handle(
       GetMyOrdersQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            var (items, total) = await uow.Orders.GetByBuyerPagedAsync(
                cu.UserId.Value, req.Page, req.PageSize, ct);
            var dtos = items.Select(MapExt.MapOrder).ToList();
            return ApiResponse<PaginatedResult<OrderDto>>.Ok(
                PaginatedResult<OrderDto>.Success(dtos, total, req.Page, req.PageSize));
        }
        public async Task<ApiResponse<PaginatedResult<OrderDto>>> Handle(
       GetSellerOrdersQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            if (cu.OwnedCompanyId == null)
                throw new BusinessException("You do not own a company.");

            var (items, total) = await uow.Orders.GetBySellerPagedAsync(
                cu.OwnedCompanyId.Value, req.Page, req.PageSize, ct);
            var dtos = items.Select(MapExt.MapOrder).ToList();
            return ApiResponse<PaginatedResult<OrderDto>>.Ok(
                PaginatedResult<OrderDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
