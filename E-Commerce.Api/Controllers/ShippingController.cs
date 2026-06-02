using E_Commerce.Core.Features.Shippings;
using E_Commerce.Core.Features.Shippings.Commands.Models;
using E_Commerce.Core.Features.Shippings.Queries.Models;
using E_Commerce.Service.Shipping;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/v1/shipping")]
    [ApiController]
    public class ShippingController(ISender mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateShipmentDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new CreateShipmentCommand(
                dto.OrderId, dto.RecipientName, dto.AddressLine1,
                dto.City, dto.State, dto.Country, dto.PostalCode,
                dto.Method, dto.ShippingCost, dto.AddressLine2,
                dto.PhoneNumber, dto.EstimatedDeliveryDate), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new GetShipmentDetailsQuery(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("order/{orderId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), 200)]
        public async Task<IActionResult> GetByOrder(Guid orderId, CancellationToken ct)
        {
            var r = await mediator.Send(new GetShipmentByOrderQuery(orderId), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Update shipment status. Allowed transitions:
        /// Pending → ReadyForPickup → InTransit (requires CarrierName + TrackingNumber)
        /// → OutForDelivery → Delivered (releases seller wallet funds) | Failed | Returned
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), 200)]
        public async Task<IActionResult> UpdateStatus(
            Guid id, [FromBody] UpdateShipmentStatusDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new UpdateShipmentStatusCommand(
                id, dto.Status, dto.CarrierName, dto.TrackingNumber,
                dto.TrackingUrl, dto.FailureReason), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("{id:guid}/track")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> Track(Guid id, [FromServices] IBostaShippingService bosta, CancellationToken ct)
        {
            var shipment = await mediator.Send(new GetShipmentDetailsQuery(id), ct);
            if (!shipment.Success || shipment.Data?.BostaTrackingNumber == null)
                return BadRequest(new { message = "الشحنة غير موجودة أو غير مرتبطة ببوستا." });

            var tracking = await bosta.TrackShipmentAsync(shipment.Data.BostaTrackingNumber, ct);
            return Ok(new
            {
                bostaTrackingNumber = shipment.Data.BostaTrackingNumber,
                status = tracking?.state,
                type = tracking?.type
            });
        }
    }
}
