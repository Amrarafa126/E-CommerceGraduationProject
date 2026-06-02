using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Queries.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Orders.Queries.Handlers
{
    public class GetOrderByIdHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetOrderByIdQuery, ApiResponse<OrderDto>>
    {
        public async Task<ApiResponse<OrderDto>> Handle(GetOrderByIdQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && order.SubOrders.Any(s => s.SellerCompanyId == cu.OwnedCompanyId.Value);
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("You are not authorized to view this order.");

            var dto = mapper.Map<OrderDto>(order);
            return ApiResponse<OrderDto>.Ok(dto);
        }
    }
}
