using E_Commerce.Core.Features.ProductReview;
using E_Commerce.Core.Features.ProductReview.Commands.Models;
using E_Commerce.Core.Features.ProductReview.Queries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reviews")]
    [Produces("application/json")]
    public class ReviewsController(ISender mediator) : ControllerBase
    {
        /// <summary>Get paginated reviews for a product.</summary>
        [HttpGet("product/{productId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductReviewDto>>), 200)]
        public async Task<IActionResult> GetByProduct(
            Guid productId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetProductReviewsQuery(productId, page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Get rating statistics for a product:
        /// average, total count, and distribution per star (1–5).
        /// </summary>
        [HttpGet("product/{productId:guid}/stats")]
        [ProducesResponseType(typeof(ApiResponse<ReviewStatsDto>), 200)]
        public async Task<IActionResult> GetStats(Guid productId, CancellationToken ct)
        {
            var r = await mediator.Send(new GetReviewStatsQuery(productId), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Submit a new review for a product (Buyer only, one review per product).</summary>
        [HttpPost]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(
                new CreateReviewCommand(dto.ProductId, dto.Rating, dto.Title, dto.Comment), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Edit your own review (Buyer only).</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewDto>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(
                new UpdateReviewCommand(id, dto.Rating, dto.Title, dto.Comment), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Delete a review (Buyer who wrote it, or Admin).</summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new DeleteReviewCommand(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Seller replies to a review on their product.</summary>
        [HttpPost("{id:guid}/reply")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewDto>), 200)]
        public async Task<IActionResult> Reply(Guid id, [FromBody] SupplierReplyDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new ReplyToReviewCommand(id, dto.Reply), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
}
