using MediatR;

namespace E_Commerce.Core.Features.RFQ.Queries.Models
{
    public record GetRfqMarketplaceByIdQuery(Guid RfqId) : IRequest<ApiResponse<RfqRequestDto>>;
}
