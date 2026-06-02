using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.DashboardComapny.Queries.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.DashboardComapny.Queries.Handlers
{
    public class DashboardComapnyHandlers(AppDBContext db, ICurrentUserService cu)
         : IRequestHandler<GetDashboardQuery, ApiResponse<CompanyDashboardDto>>
    {
        public async Task<ApiResponse<CompanyDashboardDto>> Handle(GetDashboardQuery req, CancellationToken ct)
        {
            if (cu.OwnedCompanyId == null)
                throw new ForbiddenException("يمكن للبائعين فقط عرض لوحة التحكم.");

            var companyId = cu.OwnedCompanyId.Value;
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // ── Products ──────────────────────────────────────────────────────────
            var products = await db.products
                .Where(p => p.CompanyId == companyId && !p.IsDeleted)
                .Select(p => new { p.Status })
                .ToListAsync(ct);

            // ── Orders ────────────────────────────────────────────────────────────
            var orders = await db.orderSubOrders
                .Where(s => s.SellerCompanyId == companyId && !s.IsDeleted)
                .Select(s => new { s.Status, s.TotalAmount, s.CreatedAt })
                .ToListAsync(ct);

            // ── Wallet ────────────────────────────────────────────────────────────
            var wallet = await db.Wallets
                .FirstOrDefaultAsync(w => w.CompanyId == companyId, ct);

            // ── Reviews ───────────────────────────────────────────────────────────
            var ratings = await db.productReviews
                .Where(r => r.Product.CompanyId == companyId && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            // ── Chat ──────────────────────────────────────────────────────────────
            var unread = await db.messages
                .CountAsync(m => m.Conversation.CompanyId == companyId
                              && m.SenderId != m.Conversation.BuyerId
                              && !m.IsRead, ct);

            var activeConvs = await db.conversations
                .CountAsync(c => c.CompanyId == companyId && !c.IsCompanyArchived, ct);

            // ── RFQ ───────────────────────────────────────────────────────────────
            var rfqTotal = await db.rfqRequests
                .CountAsync(r => r.SellerCompanyId == companyId && !r.IsDeleted, ct);
            var rfqPending = await db.rfqRequests
                .CountAsync(r => r.SellerCompanyId == companyId && !r.IsDeleted && r.Status == RfqStatus.Pending, ct);

            // ── Top Products ──────────────────────────────────────────────────────
            var rawTopProducts = await db.orderItems
                .Where(i => i.OrderSubOrder.SellerCompanyId == companyId && !i.OrderSubOrder.IsDeleted)
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = g.First().ProductName,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(i => i.UnitPrice * i.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToListAsync(ct);

            var productIds = rawTopProducts.Select(x => x.ProductId).ToList();
            var productImages = await db.products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.MainImageUrl })
                .ToDictionaryAsync(p => p.Id, p => p.MainImageUrl, ct);

            var topProducts = rawTopProducts
                .Select(x => new TopProductDto(
                    x.ProductId,
                    x.ProductName,
                    productImages.GetValueOrDefault(x.ProductId),
                    x.OrderCount,
                    x.Revenue,
                    0))
                .ToList();

            // ── Monthly Revenue ───────────────────────────────────────────────────
            var sixAgo = now.AddMonths(-5);

            var monthly = orders
                .Where(o => o.CreatedAt >= sixAgo)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(o => o.TotalAmount), Count = g.Count() })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Select(x => new MonthlyRevenueDto(
                    new DateTime(x.Year, x.Month, 1).ToString("MMM"),
                    x.Year,
                    x.Revenue,
                    x.Count))
                .ToList();

            var statusDist = orders
                .GroupBy(o => o.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // ── Build DTO ─────────────────────────────────────────────────────────
            var dto = new CompanyDashboardDto(
                new OverviewDto(
                    products.Count,
                    products.Count(p => p.Status == ProductStatus.Active),
                    orders.Count,
                    orders.Count(o => o.Status == OrderStatus.Pending),
                    orders.Count(o => o.Status == OrderStatus.Processing)),
                new FinancialDto(
                    orders.Sum(o => o.TotalAmount),
                    orders.Where(o => o.CreatedAt >= startOfMonth).Sum(o => o.TotalAmount),
                    wallet?.PendingBalance ?? 0,
                    wallet?.AvailableBalance ?? 0),
                new ReviewSummaryDto(
                    ratings.Any() ? Math.Round(ratings.Average(), 1) : 0,
                    ratings.Count),
                new ChatSummaryDto(unread, activeConvs),
                new RfqSummaryDto(rfqPending, rfqTotal),
                topProducts,
                monthly,
                statusDist);

            return ApiResponse<CompanyDashboardDto>.Ok(dto);
        }
    }
}
