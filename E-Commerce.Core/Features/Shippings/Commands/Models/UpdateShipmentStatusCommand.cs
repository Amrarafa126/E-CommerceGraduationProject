using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings.Commands.Models
{
    public record UpdateShipmentStatusCommand(
       Guid ShipmentId, int Status,
       string? CarrierName = null, string? TrackingNumber = null,
       string? TrackingUrl = null, string? FailureReason = null)
       : IRequest<ApiResponse<ShipmentDto>>;
}
