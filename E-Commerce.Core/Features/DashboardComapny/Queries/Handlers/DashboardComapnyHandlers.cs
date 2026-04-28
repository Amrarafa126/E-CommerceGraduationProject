using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.DashboardComapny.Queries.Models;
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

namespace E_Commerce.Core.Features.DashboardComapny.Queries.Handlers
{
    public class DashboardComapnyHandlers(AppDBContext db, ICurrentUserService cu)
    : IRequestHandler<GetDashboardQuery, ApiResponse<CompanyDashboardDto>>
    {
        public async Task<ApiResponse<CompanyDashboardDto>> Handle(GetDashboardQuery req, CancellationToken ct)
        {
            if (cu.OwnedCompanyId == null) throw new ForbiddenException("Only sellers can view dashboard.");

            var companyId = cu.OwnedCompanyId.Value;
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // ── Products ─────────────────────────────────────────────
            var products = await db.products
                .Where(p => p.CompanyId == companyId && !p.IsDeleted)
                .Select(p => new { p.Status }).ToListAsync(ct);

            // ── Orders ────────────────────────────────────────────────
            var orders = await db.orders
                .Where(o => o.SellerCompanyId == companyId && !o.IsDeleted)
                .Select(o => new { o.Status, o.TotalAmount, o.CreatedAt })
                .ToListAsync(ct);

            // ── Wallet ────────────────────────────────────────────────
            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.CompanyId == companyId, ct);

            // ── Reviews ───────────────────────────────────────────────
            var ratings = await db.productReviews
                .Where(r => r.Product.CompanyId == companyId && !r.IsDeleted)
                .Select(r => r.Rating).ToListAsync(ct);

            // ── Chat ──────────────────────────────────────────────────
            var unread = await db.messages
                .CountAsync(m => m.Conversation.CompanyId == companyId
                              && m.SenderId != m.Conversation.BuyerId && !m.IsRead, ct);
            var activeConvs = await db.conversations
                .CountAsync(c => c.CompanyId == companyId && !c.IsCompanyArchived, ct);

            // ── RFQ ───────────────────────────────────────────────────
            var rfqStats = await db.rfqRequests
                .Where(r => r.SellerCompanyId == companyId && !r.IsDeleted)
                .GroupBy(_ => true)
                .Select(g => new { Total = g.Count(), Pending = g.Count(r => r.Status == RfqStatus.Pending) })
                .FirstOrDefaultAsync(ct);

            // ── Top Products (last 90 days) ───────────────────────────
            var topProducts = await db.orderItems
                .Where(i => i.Order.SellerCompanyId == companyId && !i.Order.IsDeleted)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new TopProductDto(
                    g.Key.ProductId, g.Key.ProductName, null,
                    g.Count(), g.Sum(i => i.UnitPrice * i.Quantity), 0))
                .OrderByDescending(x => x.Revenue)
                .Take(5).ToListAsync(ct);

            // ── Monthly Revenue ───────────────────────────────────────
            var sixAgo = now.AddMonths(-5);
            var monthly = orders
                .Where(o => o.CreatedAt >= sixAgo)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDto(
                    new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    g.Key.Year, g.Sum(o => o.TotalAmount), g.Count()))
                .OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();

            var statusDist = orders
                .GroupBy(o => o.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

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
                new RfqSummaryDto(rfqStats?.Pending ?? 0, rfqStats?.Total ?? 0),
                topProducts,
                monthly,
                statusDist);

            return ApiResponse<CompanyDashboardDto>.Ok(dto);
        }
    }
}
