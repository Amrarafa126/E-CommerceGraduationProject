using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetTopSellersHandler(AppDBContext db, ICurrentUserService cu)
        : IRequestHandler<GetTopSellersQuery, ApiResponse<List<TopSellerDto>>>
    {
        public async Task<ApiResponse<List<TopSellerDto>>> Handle(
            GetTopSellersQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var result = await db.orderSubOrders
                .Where(s => !s.IsDeleted && s.Status == OrderStatus.Completed)
                .GroupBy(s => new { s.SellerCompanyId, s.SellerCompany.CompanyName })
                .Select(g => new
                {
                    g.Key.SellerCompanyId,
                    g.Key.CompanyName,
                    Revenue = g.Sum(s => s.TotalAmount),
                    OrderCount = g.Count(),
                })
                .OrderByDescending(x => x.Revenue)
                .Take(Math.Clamp(req.Limit, 1, 50))
                .ToListAsync(ct);

            var productCounts = await db.products
                .Where(p => result.Select(r => r.SellerCompanyId).Contains(p.CompanyId) && !p.IsDeleted)
                .GroupBy(p => p.CompanyId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

            var dtos = result.Select((r, i) => new TopSellerDto(
                i + 1, r.SellerCompanyId, r.CompanyName,
                r.Revenue, r.OrderCount,
                productCounts.GetValueOrDefault(r.SellerCompanyId))).ToList();

            return ApiResponse<List<TopSellerDto>>.Ok(dtos);
        }
    }
}
