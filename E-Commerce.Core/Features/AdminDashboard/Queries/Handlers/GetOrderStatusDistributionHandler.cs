using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetOrderStatusDistributionHandler(AppDBContext db, ICurrentUserService cu)
    : IRequestHandler<GetOrderStatusDistributionQuery, ApiResponse<List<OrderStatusCountDto>>>
    {
        public async Task<ApiResponse<List<OrderStatusCountDto>>> Handle(
            GetOrderStatusDistributionQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var dist = await db.orders
                .Where(o => !o.IsDeleted)
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Revenue = g.Sum(o => o.TotalAmount) })
                .ToListAsync(ct);

            var result = dist
                .OrderBy(d => d.Status)
                .Select(d => new OrderStatusCountDto(d.Status.ToString(), d.Count, d.Revenue))
                .ToList();

            return ApiResponse<List<OrderStatusCountDto>>.Ok(result);
        }
    }
}
