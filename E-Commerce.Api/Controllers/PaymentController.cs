using E_Commerce.Core.Features.Payment;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    [Authorize]
    [Produces("application/json")]
    public class PaymentController(ISender mediator ) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 402)]
        public async Task<IActionResult> Pay([FromBody] PayRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(
                new InitiatePaymentCommand(req.OrderId, req.PaymentMethod, req.CardToken, req.Currency), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("order/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDto>), 200)]
        public async Task<IActionResult> GetByOrder(Guid orderId, CancellationToken ct)
        {
            var r = await mediator.Send(new GetPaymentByOrderQuery(orderId), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/refund")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDto>), 200)]
        public async Task<IActionResult> Refund(Guid id, [FromBody] RefundReq req, CancellationToken ct)
        {
            var r = await mediator.Send(new RefundPaymentCommand(id, req.Amount, req.Reason), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get payment details for an order</summary>
    }
    public record PayRequest(Guid OrderId, int PaymentMethod, string? CardToken, string Currency = "EGP");
    public record RefundReq(decimal Amount, string Reason);

}

