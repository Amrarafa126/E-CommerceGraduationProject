using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var endMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);
            var startMonth = endMonth.AddMonths(-months);

            // Single grouped query for sub-orders
            var orderData = await db.orderSubOrders
                .Where(s => !s.IsDeleted && s.Status == OrderStatus.Completed
                    && s.CreatedAt >= startMonth && s.CreatedAt < endMonth)
                .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(s => (decimal?)s.TotalAmount) ?? 0,
                    Orders = g.Count()
                })
                .ToDictionaryAsync(x => (x.Year, x.Month), ct);

            // Single grouped query for users
            var userData = await db.Users
                .Where(u => !u.IsDeleted && u.CreatedAt >= startMonth && u.CreatedAt < endMonth)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToDictionaryAsync(x => (x.Year, x.Month), x => x.Count, ct);

            var result = new List<RevenueChartPointDto>();
            for (int i = months - 1; i >= 0; i--)
            {
                var start = endMonth.AddMonths(-i - 1);
                var key = (start.Year, start.Month);
                orderData.TryGetValue(key, out var orderPoint);
                userData.TryGetValue(key, out var userCount);

                result.Add(new RevenueChartPointDto(
                    start.ToString("yyyy-MM"),
                    start.ToString("MMM yyyy"),
                    orderPoint?.Revenue ?? 0,
                    orderPoint?.Orders ?? 0,
                    userCount));
            }

            return ApiResponse<List<RevenueChartPointDto>>.Ok(result);
        }
    }
}
