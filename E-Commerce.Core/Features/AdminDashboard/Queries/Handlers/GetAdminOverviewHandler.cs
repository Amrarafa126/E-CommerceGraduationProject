using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetAdminOverviewHandler(
    AppDBContext db,
    ICurrentUserService cu)
    : IRequestHandler<GetAdminOverviewQuery, ApiResponse<AdminOverviewDto>>
    {
        public async Task<ApiResponse<AdminOverviewDto>> Handle(
       GetAdminOverviewQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var now = DateTime.UtcNow;
            var today = now.Date;
            var thisMonth = new DateTime(now.Year, now.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);
            var thisYear = new DateTime(now.Year, 1, 1);

            // ── Users ──────────────────────────────────────────────
            var totalUsers = await db.Users.CountAsync(u => !u.IsDeleted, ct);
            var totalSellers = await db.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Seller, ct);
            var totalBuyers = await db.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Buyer, ct);
            var newUsersToday = await db.Users.CountAsync(u => u.CreatedAt >= today && !u.IsDeleted, ct);
            var newUsersMonth = await db.Users.CountAsync(u => u.CreatedAt >= thisMonth && !u.IsDeleted, ct);

            // ── Companies ──────────────────────────────────────────
            var totalCompanies = await db.companies.CountAsync(c => !c.IsDeleted, ct);
            var activeCompanies = await db.companies.CountAsync(c => c.Status == CompanyStatus.Active && !c.IsDeleted, ct);
            var pendingCompanies = await db.companies.CountAsync(c => c.Status == CompanyStatus.Pending && !c.IsDeleted, ct);
            var newCompaniesMonth = await db.companies.CountAsync(c => c.CreatedAt >= thisMonth && !c.IsDeleted, ct);

            // ── Products ───────────────────────────────────────────
            var totalProducts = await db.products.CountAsync(p => !p.IsDeleted, ct);
            var activeProducts = await db.products.CountAsync(p => p.Status == ProductStatus.Active && !p.IsDeleted, ct);
            var draftProducts = await db.products.CountAsync(p => p.Status == ProductStatus.Draft && !p.IsDeleted, ct);

            // ── Orders ─────────────────────────────────────────────
            var totalOrders = await db.orders.CountAsync(o => !o.IsDeleted, ct);
            var ordersToday = await db.orders.CountAsync(o => o.CreatedAt >= today && !o.IsDeleted, ct);
            var ordersMonth = await db.orders.CountAsync(o => o.CreatedAt >= thisMonth && !o.IsDeleted, ct);
            var pendingOrders = await db.orderSubOrders.CountAsync(s => s.Status == OrderStatus.Pending && !s.IsDeleted, ct);

            // ── Revenue ────────────────────────────────────────────
            var revenueTotal = await db.orderSubOrders
                .Where(s => s.Status == OrderStatus.Completed && !s.IsDeleted)
                .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

            var revenueToday = await db.orderSubOrders
                .Where(s => s.Status == OrderStatus.Completed && s.CreatedAt >= today && !s.IsDeleted)
                .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

            var revenueMonth = await db.orderSubOrders
                .Where(s => s.Status == OrderStatus.Completed && s.CreatedAt >= thisMonth && !s.IsDeleted)
                .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

            var revenueLastMonth = await db.orderSubOrders
                .Where(s => s.Status == OrderStatus.Completed
                    && s.CreatedAt >= lastMonth && s.CreatedAt < thisMonth && !s.IsDeleted)
                .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

            var revenueYear = await db.orderSubOrders
                .Where(s => s.Status == OrderStatus.Completed && s.CreatedAt >= thisYear && !s.IsDeleted)
                .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

            var totalReviews = await db.productReviews.CountAsync(r => !r.IsDeleted, ct);
            var avgRating = await db.productReviews.Where(r => !r.IsDeleted)
                .AverageAsync(r => (double?)r.Rating, ct) ?? 0;
            var totalRfqs = await db.rfqRequests.CountAsync(r => !r.IsDeleted, ct);
            var pendingRfqs = await db.rfqRequests.CountAsync(r => r.Status == RfqStatus.Pending && !r.IsDeleted, ct);

            double revenueGrowth = revenueLastMonth == 0 ? 100
                : Math.Round((double)((revenueMonth - revenueLastMonth) / revenueLastMonth * 100), 1);

            return ApiResponse<AdminOverviewDto>.Ok(new AdminOverviewDto(
                // Users
                totalUsers, totalSellers, totalBuyers, newUsersToday, newUsersMonth,
                // Companies
                totalCompanies, activeCompanies, pendingCompanies, newCompaniesMonth,
                // Products
                totalProducts, activeProducts, draftProducts,
                // Orders
                totalOrders, ordersToday, ordersMonth, pendingOrders,
                // Revenue
                revenueTotal, revenueToday, revenueMonth, revenueLastMonth, revenueYear, revenueGrowth,
                // Engagement
                totalReviews, Math.Round(avgRating, 2), totalRfqs, pendingRfqs,
                now));
        }
    }
}
