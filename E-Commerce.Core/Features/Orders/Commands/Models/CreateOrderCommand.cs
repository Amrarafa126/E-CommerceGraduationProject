using E_Commerce.Data.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raven.Client.Constants.Documents.PeriodicBackup;

namespace E_Commerce.Core.Features.Orders.Commands.Models
{
    public record CreateOrderCommand(
     Guid SellerCompanyId, string? Notes, string Currency,
     List<CreateOrderItemDto> Items)
     : IRequest<ApiResponse<OrderDto>>;

}

