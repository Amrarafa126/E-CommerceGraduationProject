using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.Shippings.Commands.Handlers
{
    public class ShippingHandlersCommands(IUnitOfWork uow, ICurrentUserService cu)
    : IRequestHandler<CreateShipmentCommand, ApiResponse<ShipmentDto>>,
         IRequestHandler<UpdateShipmentStatusCommand, ApiResponse<ShipmentDto>>
    {

        public async Task<ApiResponse<ShipmentDto>> Handle(CreateShipmentCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            // OwnedCompanyId from JWT — no extra DB call
            if (cu.OwnedCompanyId != order.SellerCompanyId && cu.Role != "Admin")
                throw new ForbiddenException("Only the seller can create shipments for this order.");

            if (order.Status != OrderStatus.Paid && order.Status != OrderStatus.Processing)
                throw new BusinessException($"Cannot create shipment for order in status '{order.Status}'.");

            if (order.ShipmentId.HasValue)
                throw new ConflictException("Shipment already exists for this order.");

            if (!Enum.TryParse<ShippingMethod>(req.Method, true, out var method))
                throw new BusinessException("Invalid shipping method.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var shipment = Shipping.Create(
                    order.Id, req.RecipientName,
                    req.AddressLine1, req.City, req.State, req.Country, req.PostalCode,
                    method, req.ShippingCost, "USD",
                    req.AddressLine2, req.PhoneNumber, req.EstimatedDeliveryDate);

                await uow.shipping.AddAsync(shipment, ct);
                await uow.SaveChangesAsync(ct);

                order.LinkShipment(shipment.Id);
                if (order.Status == OrderStatus.Paid)
                    order.MarkProcessing();

                order.RecalculateTotals(req.ShippingCost);
                uow.Orders.Update(order);

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                return ApiResponse<ShipmentDto>.Created(ShippingMapper.MapShipment(shipment));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
        public async Task<ApiResponse<ShipmentDto>> Handle(
       UpdateShipmentStatusCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var shipment = await uow.shipping.GetWithEventsAsync(req.ShipmentId, ct)
                ?? throw new NotFoundException(nameof(Shipping), req.ShipmentId);

            var order = await uow.Orders.GetWithFullDetailsAsync(shipment.OrderId, ct)!;
            // OwnedCompanyId from JWT — no extra DB call
            if (cu.OwnedCompanyId != order?.SellerCompanyId && cu.Role != "Admin")
                throw new ForbiddenException("Only the seller or admin can update shipment status.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                // Apply status transition on shipment
                switch (req.Status)
                {
                    case "ReadyForPickup":
                        shipment.MarkReadyForPickup();
                        break;
                    case "InTransit":
                        shipment.MarkInTransit(req.CarrierName!, req.TrackingNumber!, req.TrackingUrl);
                        order?.MarkShipped();
                        break;
                    case "OutForDelivery":
                        shipment.MarkOutForDelivery();
                        break;
                    case "Delivered":
                        shipment.MarkDelivered();
                        order?.MarkDelivered();
                        // Release wallet balance to Available
                        if (order != null)
                        {
                            var wallet = await uow.Wallets.GetByCompanyAsync(order.SellerCompanyId, ct);
                            if (wallet != null)
                            {
                                var releaseAmount = order.TotalAmount;
                                wallet.ReleasePending(Math.Min(releaseAmount, wallet.PendingBalance));
                                uow.Wallets.Update(wallet);
                            }
                        }
                        break;
                    case "Failed":
                        shipment.MarkFailed(req.FailureReason ?? "Delivery failed.");
                        break;
                    case "Returned":
                        shipment.MarkReturned();
                        break;
                }

                uow.shipping.Update(shipment);
                if (order != null) uow.Orders.Update(order);

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                return ApiResponse<ShipmentDto>.Ok(ShippingMapper.MapShipment(shipment));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
