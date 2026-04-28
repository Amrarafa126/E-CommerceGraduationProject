using E_Commerce.Core.Features.RFQ;
using E_Commerce.Core.Features.RFQ.Commands.Models;
using E_Commerce.Core.Features.RFQ.Queries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/rfq")]
    [Authorize]
    [Produces("application/json")]
    public class RFQController(ISender mediator) : ControllerBase
    {
        /// <summary>Get a single RFQ with all quotes.</summary>
        [HttpGet("Get-All-Quotes/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new GetRfqByIdQuery(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get all RFQs sent by the authenticated buyer (paginated).</summary>
        [HttpGet("my-rfqs")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RfqRequestDto>>), 200)]
        public async Task<IActionResult> GetMine(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetMyRfqsQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get all RFQs received by the authenticated seller's company (paginated).</summary>
        [HttpGet("seller")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RfqRequestDto>>), 200)]
        public async Task<IActionResult> GetSellerRfqs(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetSellerRfqsQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Buyer sends a Request for Quotation to a seller company.
        /// Optionally link to a specific product and set a target price.
        /// </summary>
        [HttpPost("send-rfq")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateRfqDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new CreateRfqCommand(
                dto.Title, dto.Description, dto.Quantity, dto.SellerCompanyId,
                dto.Currency, dto.TargetPrice, dto.ShippingCountry,
                dto.DeadlineDate, dto.ProductId), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Buyer cancels an open RFQ.</summary>
        [HttpPost("{id:guid}/cancel")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 200)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new CancelRfqCommand(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/quote")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<RfqQuoteDto>), 201)]
        public async Task<IActionResult> SubmitQuote(
            Guid id, [FromBody] SubmitQuoteRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new SubmitQuoteCommand(
                id, req.UnitPrice, req.Quantity, req.Currency,
                req.Notes, req.PaymentTerms, req.DeliveryTerms, req.ValidityDays), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("quotes/{quoteId:guid}/accept")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 200)]
        public async Task<IActionResult> AcceptQuote(Guid quoteId, CancellationToken ct)
        {
            var r = await mediator.Send(new AcceptQuoteCommand(quoteId), ct);
            return StatusCode(r.StatusCode, r);
        }
    }

    public record SubmitQuoteRequest(
        decimal UnitPrice, int Quantity, string Currency = "EGP",
        string? Notes = null, string? PaymentTerms = null,
        string? DeliveryTerms = null, int ValidityDays = 7);
}

