using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{

    public record SubmitQuoteCommand(
        Guid RfqRequestId, decimal UnitPrice, int Quantity, string Currency = "EGP",
        string? Notes = null, string? PaymentTerms = null, string? DeliveryTerms = null, int ValidityDays = 7)
        : IRequest<ApiResponse<RfqQuoteDto>>;
}
