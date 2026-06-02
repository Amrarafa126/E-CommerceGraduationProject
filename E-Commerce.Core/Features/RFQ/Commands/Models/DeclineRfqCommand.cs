using MediatR;
using System;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record DeclineRfqCommand(Guid RfqId) : IRequest<ApiResponse<RfqRequestDto>>;
}
