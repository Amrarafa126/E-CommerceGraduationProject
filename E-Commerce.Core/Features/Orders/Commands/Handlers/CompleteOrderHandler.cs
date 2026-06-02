using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Orders.Commands.Handlers
{
    public class CompleteOrderHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<CompleteOrderCommand, ApiResponse<OrderDto>>
    {
        public async Task<ApiResponse<OrderDto>> Handle(CompleteOrderCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isAdmin)
                throw new ForbiddenException("Only the buyer or an admin can mark an order as completed.");

            foreach (var sub in order.SubOrders)
            {
                if (sub.Status == Data.Status.OrderStatus.Delivered)
                    sub.MarkCompleted();
            }

            order.RecalculateTotals();
            await uow.SaveChangesAsync(ct);

            var dto = mapper.Map<OrderDto>(order);
            return ApiResponse<OrderDto>.Ok(dto);
        }
    }
}
