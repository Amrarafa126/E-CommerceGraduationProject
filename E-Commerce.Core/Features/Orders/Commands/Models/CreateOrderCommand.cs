using E_Commerce.Core.Features.Orders;
using MediatR;

namespace E_Commerce.Core.Features.Orders.Commands.Models
{
    public record CreateOrderCommand(
        Guid SellerCompanyId,
        string? Notes,
        string Currency,
        List<CreateOrderItemDto> Items,
        ShippingAddressDto ShippingAddress,
        int ShippingMethod,
        string? IdempotencyKey = null,
        string? PoNumber = null)
        : IRequest<ApiResponse<OrderDto>>;
}
