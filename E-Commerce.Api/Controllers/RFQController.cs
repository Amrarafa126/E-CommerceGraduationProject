using E_Commerce.Core.Features.RFQ;
using E_Commerce.Core.Features.RFQ.Commands.Models;
using E_Commerce.Core.Features.RFQ.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Status;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/rfqs")]
    [Authorize]
    [Produces("application/json")]
    public class RFQController(ISender mediator) : ControllerBase
    {
        /// <summary>Get a single RFQ with all quotes.</summary>
        [HttpGet("{id:guid}")]
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
            [FromQuery] int? status = null,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetSellerRfqsQuery(page, pageSize, status), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Browse public RFQ marketplace (sellers only).
        /// </summary>
        [HttpGet("marketplace")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RfqRequestDto>>), 200)]
        public async Task<IActionResult> GetMarketplace(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] string? country = null,
            [FromQuery] int? status = null,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetRfqMarketplaceQuery(page, pageSize, search, categoryId, country, status), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get a public marketplace RFQ detail (sellers only).</summary>
        [HttpGet("marketplace/{id:guid}")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 200)]
        public async Task<IActionResult> GetMarketplaceById(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new GetRfqMarketplaceByIdQuery(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Buyer sends a Request for Quotation.
        /// Seller company is optional; if omitted the RFQ is published to the public marketplace.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateRfqDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new CreateRfqCommand(
                dto.Title,
                dto.Description,
                dto.Quantity,
                dto.SellerCompanyId,
                dto.Currency ?? "EGP",
                (UnitOfMeasure)dto.UnitOfMeasure,
                dto.CategoryId,
                dto.TargetPrice,
                dto.ShippingCountry,
                dto.DestinationCity,
                dto.DestinationCountry,
                (ShippingMethod?)dto.PreferredShippingMethod,
                dto.PaymentTerms,
                dto.RequiredCertifications,
                dto.SupplierRequirements,
                dto.DeadlineDate,
                dto.ProductId,
                dto.Attachments,
                dto.IsPublic), ct);
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

        [HttpPost("{id:guid}/decline")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 200)]
        public async Task<IActionResult> DeclineRfq(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new DeclineRfqCommand(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/quote")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<RfqQuoteDto>), 201)]
        public async Task<IActionResult> SubmitQuote(
            Guid id, [FromBody] SubmitQuoteRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new SubmitQuoteCommand(
                id,
                req.SellerCompanyId,
                req.UnitPrice,
                req.Quantity,
                req.Currency,
                req.Notes,
                req.PaymentTerms,
                req.DeliveryTerms,
                req.ValidityDays,
                req.LeadTimeDays,
                req.SampleAvailable), ct);
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

        [HttpPost("quotes/{quoteId:guid}/decline")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<RfqRequestDto>), 200)]
        public async Task<IActionResult> DeclineQuote(Guid quoteId, CancellationToken ct)
        {
            var r = await mediator.Send(new DeclineQuoteCommand(quoteId), ct);
            return StatusCode(r.StatusCode, r);
        }
    }

    public record SubmitQuoteRequest(
        Guid SellerCompanyId,
        decimal UnitPrice,
        int Quantity,
        string Currency = "EGP",
        string? Notes = null,
        string? PaymentTerms = null,
        string? DeliveryTerms = null,
        int ValidityDays = 7,
        int? LeadTimeDays = null,
        bool SampleAvailable = false);
}
