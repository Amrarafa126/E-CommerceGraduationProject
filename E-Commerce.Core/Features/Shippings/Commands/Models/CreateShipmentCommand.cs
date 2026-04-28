using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings.Commands.Models
{
    public record CreateShipmentCommand(
       Guid OrderId, string RecipientName,
       string AddressLine1, string City, string State,
       string Country, string PostalCode, string Method,
       decimal ShippingCost,
       string? AddressLine2 = null, string? PhoneNumber = null,
       DateTime? EstimatedDeliveryDate = null)
       : IRequest<ApiResponse<ShipmentDto>>;
}
