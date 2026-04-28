using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Queries.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings.Queries.Handlers
{
    public class ShippingHandlersQueries(IUnitOfWork uow)
    : IRequestHandler<GetShipmentByOrderQuery, ApiResponse<ShipmentDto>>,
        IRequestHandler<GetShipmentDetailsQuery, ApiResponse<ShipmentDto>>
    {
        public async Task<ApiResponse<ShipmentDto>> Handle(
        GetShipmentByOrderQuery req, CancellationToken ct)
        {
            var shipment = await uow.shipping.GetByOrderIdAsync(req.OrderId, ct)
                ?? throw new NotFoundException("Shipment for order", req.OrderId);

            return ApiResponse<ShipmentDto>.Ok(ShippingMapper.MapShipment(shipment));
        }

        public async Task<ApiResponse<ShipmentDto>> Handle(
        GetShipmentDetailsQuery req, CancellationToken ct)
        {
            var shipment = await uow.shipping.GetWithEventsAsync(req.ShipmentId, ct)
                ?? throw new NotFoundException(nameof(Shipping), req.ShipmentId);

            return ApiResponse<ShipmentDto>.Ok(ShippingMapper.MapShipment(shipment));
        }
    }
}
