using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Queries.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Shippings.Queries.Handlers
{
    public class GetShipmentByOrderHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetShipmentByOrderQuery, ApiResponse<ShipmentDto>>
    {
        public async Task<ApiResponse<ShipmentDto>> Handle(GetShipmentByOrderQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && order.SubOrders.Any(s => s.SellerCompanyId == cu.OwnedCompanyId.Value);
            bool isAdmin = cu.Role == "Admin";
            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("You can only view shipments for your own orders.");

            // For backward compatibility, find shipment via the first sub-order
            var subOrder = order.SubOrders.FirstOrDefault();
            if (subOrder == null)
                throw new NotFoundException("Shipment for order", req.OrderId);

            var shipment = await uow.shipping.GetByOrderSubOrderIdAsync(subOrder.Id, ct)
                ?? throw new NotFoundException("Shipment for order", req.OrderId);

            var dto = mapper.Map<ShipmentDto>(shipment);
            return ApiResponse<ShipmentDto>.Ok(dto);
        }
    }
}
