using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Commands.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Shippings.Commands.Handlers
{
    public class UpdateShipmentStatusHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<UpdateShipmentStatusCommand, ApiResponse<ShipmentDto>>
    {
        public async Task<ApiResponse<ShipmentDto>> Handle(UpdateShipmentStatusCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var shipment = await uow.shipping.GetWithEventsAsync(req.ShipmentId, ct)
                ?? throw new NotFoundException(nameof(Data.Entity.Shipping), req.ShipmentId);

            var order = await uow.Orders.GetWithFullDetailsAsync(shipment.OrderSubOrderId, ct);
            var subOrder = order?.SubOrders.FirstOrDefault(s => s.Id == shipment.OrderSubOrderId);

            if (cu.OwnedCompanyId != subOrder?.SellerCompanyId && cu.Role != "Admin")
                throw new ForbiddenException("Only the seller or admin can update shipment status.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var status = (ShippingStatus)(req.Status + 1);
                if (!Enum.IsDefined(typeof(ShippingStatus), status))
                    throw new BusinessException("Invalid shipment status.");

                switch (status)
                {
                    case ShippingStatus.ReadyForPickup:
                        shipment.MarkReadyForPickup();
                        break;
                    case ShippingStatus.InTransit:
                        shipment.MarkInTransit(req.CarrierName!, req.TrackingNumber!, req.TrackingUrl);
                        subOrder?.MarkShipped();
                        break;
                    case ShippingStatus.OutForDelivery:
                        shipment.MarkOutForDelivery();
                        break;
                    case ShippingStatus.Delivered:
                        shipment.MarkDelivered();
                        subOrder?.MarkDelivered();
                        if (subOrder != null)
                        {
                            var wallet = await uow.Wallets.GetByCompanyAsync(subOrder.SellerCompanyId, ct);
                            if (wallet != null)
                            {
                                var releaseAmount = subOrder.TotalAmount;
                                wallet.ReleasePending(Math.Min(releaseAmount, wallet.PendingBalance));
                            }
                        }
                        break;
                    case ShippingStatus.Failed:
                        shipment.MarkFailed(req.FailureReason ?? "Delivery failed.");
                        break;
                    case ShippingStatus.Returned:
                        shipment.MarkReturned();
                        break;
                }

                order?.RecalculateTotals();

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var dto = mapper.Map<ShipmentDto>(shipment);
                return ApiResponse<ShipmentDto>.Ok(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
