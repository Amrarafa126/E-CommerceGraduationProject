using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Queries.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Shippings.Queries.Handlers
{
    public class GetShipmentDetailsHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetShipmentDetailsQuery, ApiResponse<ShipmentDto>>
    {
        public async Task<ApiResponse<ShipmentDto>> Handle(GetShipmentDetailsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var shipment = await uow.shipping.GetWithEventsAsync(req.ShipmentId, ct)
                ?? throw new NotFoundException(nameof(Shipping), req.ShipmentId);

            var order = await uow.Orders.GetWithFullDetailsAsync(shipment.OrderSubOrderId, ct)
                ?? throw new NotFoundException(nameof(Order), shipment.OrderSubOrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && order.SubOrders.Any(s => s.SellerCompanyId == cu.OwnedCompanyId.Value);
            bool isAdmin = cu.Role == "Admin";
            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("You can only view shipments for your own orders.");

            var dto = mapper.Map<ShipmentDto>(shipment);
            return ApiResponse<ShipmentDto>.Ok(dto);
        }
    }
}
