using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record CreateRfqCommand(
     string Title, string Description, int Quantity, Guid SellerCompanyId,
     string Currency = "USD", string? TargetPrice = null,
     string? ShippingCountry = null, DateTime? DeadlineDate = null, Guid? ProductId = null)
     : IRequest<ApiResponse<RfqRequestDto>>;
}
