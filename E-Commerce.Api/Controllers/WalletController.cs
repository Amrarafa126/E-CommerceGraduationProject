using E_Commerce.Core.Features.Payment;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/wallets")]
    [Authorize]
    [Produces("application/json")]
    public class WalletController(ISender mediator) : ControllerBase
    {
        /// <summary>View seller's company wallet balance</summary>
        [HttpGet("balance")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<WalletDto>), 200)]
        public async Task<IActionResult> Balance(CancellationToken ct)
        {
            var r = await mediator.Send(new GetWalletQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("transactions")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PayoutDto>>), 200)]
        public async Task<IActionResult> Transactions(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetPayoutsQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("withdraw")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PayoutDto>), 201)]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new RequestPayoutCommand(req.Amount, req.BankAccountLast4), ct);
            return StatusCode(r.StatusCode, r);
        }
    }

    public record WithdrawRequest(decimal Amount, string? BankAccountLast4 = null);
}
