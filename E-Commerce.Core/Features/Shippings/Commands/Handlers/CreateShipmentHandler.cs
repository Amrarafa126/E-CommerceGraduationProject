using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.ValueObjects;
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
                var savedAddress = order.ShippingAddress;
                var recipientName = !string.IsNullOrWhiteSpace(req.RecipientName) ? req.RecipientName : savedAddress?.RecipientName ?? "";
                var addressLine1 = !string.IsNullOrWhiteSpace(req.AddressLine1) ? req.AddressLine1 : savedAddress?.AddressLine1 ?? "";
                var addressLine2 = !string.IsNullOrWhiteSpace(req.AddressLine2) ? req.AddressLine2 : savedAddress?.AddressLine2;
                var city = !string.IsNullOrWhiteSpace(req.City) ? req.City : savedAddress?.City ?? "";
                var state = !string.IsNullOrWhiteSpace(req.State) ? req.State : savedAddress?.State ?? "";
                var country = !string.IsNullOrWhiteSpace(req.Country) ? req.Country : savedAddress?.Country ?? "";
                var postalCode = !string.IsNullOrWhiteSpace(req.PostalCode) ? req.PostalCode : savedAddress?.PostalCode ?? "";
                var phone = !string.IsNullOrWhiteSpace(req.PhoneNumber) ? req.PhoneNumber : savedAddress?.PhoneNumber;

                var shipment = Data.Entity.Shipping.Create(
                    subOrder.Id, recipientName,
                    addressLine1, city, state, country, postalCode,
                    method, req.ShippingCost, "EGP",
                    addressLine2, phone, req.EstimatedDeliveryDate);

                // Get seller company for pickup address
                var sellerCompany = await uow.Companies.GetByIdAsync(subOrder.SellerCompanyId, ct)
                    ?? throw new NotFoundException(nameof(Company), subOrder.SellerCompanyId);

                // Create Bosta delivery
                var codAmount = subOrder.Payment?.Method == PaymentMethod.CashOnDelivery ? subOrder.TotalAmount : 0;
                var bostaResult = await bosta.CreateShipmentAsync(
                    recipientName,
                    addressLine1,
                    city,
                    state,
                    country,
                    postalCode,
                    phone,
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
