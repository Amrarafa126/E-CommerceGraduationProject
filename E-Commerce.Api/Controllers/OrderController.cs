using E_Commerce.Core.Features.Orders;
using E_Commerce.Core.Features.Orders.Commands.Models;
using E_Commerce.Core.Features.Orders.Queries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(ISender mediator) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
       => StatusCode((await mediator.Send(new GetOrderByIdQuery(id), ct)).StatusCode,
           await mediator.Send(new GetOrderByIdQuery(id), ct));

        [HttpGet("my")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDto>>), 200)]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetMyOrdersQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("seller")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDto>>), 200)]
        public async Task<IActionResult> GetSellerOrders(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetSellerOrdersQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>
        /// Place a new order. After creating, call POST /api/v1/payments to pay.
        /// Order lifecycle: Created(Pending) → Paid → Processing → Shipped → Delivered → Completed
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(
                new CreateOrderCommand(dto.SellerCompanyId, dto.Notes, dto.Currency, dto.Items), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/complete")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new CompleteOrderCommand(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new CancelOrderCommand(id, req.Reason), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
    public record CancelRequest(string Reason);
}

