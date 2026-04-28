using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings.Queries.Models
{
    public record GetShipmentByOrderQuery(Guid OrderId) : IRequest<ApiResponse<ShipmentDto>>;

}
