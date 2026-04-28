using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record CancelRfqCommand(Guid RfqId) : IRequest<ApiResponse<RfqRequestDto>>;

}
