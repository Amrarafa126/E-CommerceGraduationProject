using MediatR;

namespace E_Commerce.Core.Features.Orders.Commands.Models
{
    public record CreateOrderCommand(
        Guid SellerCompanyId,
        string? Notes,
        string Currency,
        List<CreateOrderItemDto> Items,
        string? IdempotencyKey = null,
        string? PoNumber = null)
        : IRequest<ApiResponse<OrderDto>>;
}
