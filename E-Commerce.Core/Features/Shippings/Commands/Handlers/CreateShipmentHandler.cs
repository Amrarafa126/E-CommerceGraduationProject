using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Shipping;
using MediatR;

namespace E_Commerce.Core.Features.Shippings.Commands.Handlers
{
    public class CreateShipmentHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IBostaShippingService bosta,
        IMapper mapper)
        : IRequestHandler<CreateShipmentCommand, ApiResponse<ShipmentDto>>
    {
        public async Task<ApiResponse<ShipmentDto>> Handle(CreateShipmentCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            // Find the sub-order for this seller
            var subOrder = order.SubOrders.FirstOrDefault(s => s.SellerCompanyId == cu.OwnedCompanyId)
                ?? throw new ForbiddenException("Only the seller can create shipments for their sub-order.");

            if (subOrder.Status != OrderStatus.Paid && subOrder.Status != OrderStatus.Processing)
                throw new BusinessException($"Cannot create shipment for order in status '{subOrder.Status}'.");

            if (subOrder.ShipmentId.HasValue)
                throw new ConflictException("Shipment already exists for this sub-order.");

            var method = (ShippingMethod)(req.Method + 1);
            if (!Enum.IsDefined(typeof(ShippingMethod), method))
                throw new BusinessException("Invalid shipping method.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var shipment = Data.Entity.Shipping.Create(
                    subOrder.Id, req.RecipientName,
                    req.AddressLine1, req.City, req.State, req.Country, req.PostalCode,
                    method, req.ShippingCost, "EGP",
                    req.AddressLine2, req.PhoneNumber, req.EstimatedDeliveryDate);

                // Get seller company for pickup address
                var sellerCompany = await uow.Companies.GetByIdAsync(subOrder.SellerCompanyId, ct)
                    ?? throw new NotFoundException(nameof(Company), subOrder.SellerCompanyId);

                // Create Bosta delivery
                var codAmount = subOrder.Payment?.Method == PaymentMethod.CashOnDelivery ? subOrder.TotalAmount : 0;
                var bostaResult = await bosta.CreateShipmentAsync(
                    req.RecipientName,
                    req.AddressLine1,
                    req.City,
                    req.State,
                    req.Country,
                    req.PostalCode,
                    req.PhoneNumber,
                    codAmount,
                    pickupAddressLine1: sellerCompany.Address?.Street,
                    pickupCity: sellerCompany.Address?.City,
                    pickupState: sellerCompany.Address?.State,
                    ct: ct);

                shipment.SetBostaDetails(bostaResult.trackingNumber, bostaResult._id);

                await uow.shipping.AddAsync(shipment, ct);
                subOrder.LinkShipment(shipment.Id);
                if (subOrder.Status == OrderStatus.Paid)
                    subOrder.MarkProcessing();

                subOrder.RecalculateTotals(req.ShippingCost);
                order.RecalculateTotals();

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var dto = mapper.Map<ShipmentDto>(shipment);
                return ApiResponse<ShipmentDto>.Created(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
