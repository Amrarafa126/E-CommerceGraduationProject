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

        /// <summary>View seller's company wallet balance</summary>
        [HttpGet("wallet")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<WalletDto>), 200)]
        public async Task<IActionResult> Wallet(CancellationToken ct)
        {
            var r = await mediator.Send(new GetWalletQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Request payout of available balance to bank account (Seller only).
        /// Simulates an instant payout in this mock implementation.
        /// </summary>
        [HttpPost("payout")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PayoutDto>), 201)]
        public async Task<IActionResult> Payout([FromBody] PayoutReq req, CancellationToken ct)
        {
            var r = await mediator.Send(new RequestPayoutCommand(req.Amount, req.BankAccountLast4), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("payouts")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PayoutDto>>), 200)]
        public async Task<IActionResult> Payouts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetPayoutsQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }
    }

    public record PayRequest(Guid OrderId, string PaymentMethod, string? CardToken, string Currency = "USD");
    public record RefundReq(decimal Amount, string Reason);
    public record PayoutReq(decimal Amount, string? BankAccountLast4 = null);

}

