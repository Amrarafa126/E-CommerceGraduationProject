using E_Commerce.Service.Shipping;
using MediatR;

namespace E_Commerce.Core.Features.Shippings.Queries.Models
{
    public record GetShippingRateQuery(
        string City,
        string? State,
        string Country,
        int Method,
        string? PickupCity = null,
        string? PickupState = null,
        Guid? SellerCompanyId = null)
        : IRequest<ApiResponse<ShippingRateEstimate>>;
}
