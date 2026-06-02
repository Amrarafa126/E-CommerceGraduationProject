using MediatR;
using System;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record DeclineQuoteCommand(Guid QuoteId) : IRequest<ApiResponse<RfqRequestDto>>;
}
