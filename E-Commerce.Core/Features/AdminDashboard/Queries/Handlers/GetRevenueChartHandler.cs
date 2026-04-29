using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Data.Status;
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

    public class GetRevenueChartHandler(AppDBContext db, ICurrentUserService cu)
        : IRequestHandler<GetRevenueChartQuery, ApiResponse<List<RevenueChartPointDto>>>
    {
        public async Task<ApiResponse<List<RevenueChartPointDto>>> Handle(
            GetRevenueChartQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var months = Math.Clamp(req.Months, 1, 24);
            var result = new List<RevenueChartPointDto>();

            for (int i = months - 1; i >= 0; i--)
            {
                var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-i);
                var end = start.AddMonths(1);

                var revenue = await db.orders
                    .Where(o => o.Status == OrderStatus.Completed
                        && !o.IsDeleted && o.CreatedAt >= start && o.CreatedAt < end)
                    .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0;

                var orders = await db.orders
                    .CountAsync(o => !o.IsDeleted && o.CreatedAt >= start && o.CreatedAt < end, ct);

                var newUsers = await db.Users
                    .CountAsync(u => !u.IsDeleted && u.CreatedAt >= start && u.CreatedAt < end, ct);

                result.Add(new RevenueChartPointDto(
                    start.ToString("yyyy-MM"),
                    start.ToString("MMM yyyy"),
                    revenue, orders, newUsers));
            }

            return ApiResponse<List<RevenueChartPointDto>>.Ok(result);
        }
    }

}
