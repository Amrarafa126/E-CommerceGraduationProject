using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetRecentActivityHandler(AppDBContext db, ICurrentUserService cu)
     : IRequestHandler<GetRecentActivityQuery, ApiResponse<List<ActivityItemDto>>>
    {
        public async Task<ApiResponse<List<ActivityItemDto>>> Handle(
            GetRecentActivityQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("مسموح فقط للمسؤول.");
            var lim = Math.Clamp(req.Limit, 5, 50);

            var activities = new List<ActivityItemDto>();

            // Latest orders
            var orders = await db.orders.AsNoTracking()
                .Include(o => o.Buyer)
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .Take(lim).Select(o => new ActivityItemDto(
                    "order", "طلب جديد",
                    $"{o.Buyer.FullName} قدم طلباً بقيمة {o.TotalAmount:C}",
                    "📦", o.CreatedAt, o.Id, null, null))
                .ToListAsync(ct);

            // Latest users
            var users = await db.Users.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .Take(lim).Select(u => new ActivityItemDto(
                    "user", "مستخدم جديد",
                    $"{u.FirstName} {u.LastName} سجّل كـ {u.Role}",
                    "👤", u.CreatedAt, null, u.Id, null))
                .ToListAsync(ct);

            // Latest companies
            var companies = await db.companies.AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Take(lim).Select(c => new ActivityItemDto(
                    "company", "شركة جديدة",
                    $"{c.CompanyName} سجّل كـ a seller",
                    "🏢", c.CreatedAt, null, null, c.Id))
                .ToListAsync(ct);

            // Latest reviews
            var reviews = await db.productReviews.AsNoTracking()
                .Include(r => r.Buyer)
                .Include(r => r.Product)
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .Take(lim).Select(r => new ActivityItemDto(
                    "review", "تقييم جديد",
                    $"{r.Buyer.FullName} rated \"{r.Product.Name}\" {r.Rating}★",
                    "⭐", r.CreatedAt, null, null, null))
                .ToListAsync(ct);

            activities.AddRange(orders);
            activities.AddRange(users);
            activities.AddRange(companies);
            activities.AddRange(reviews);

            var sorted = activities
                .OrderByDescending(a => a.OccurredAt)
                .Take(lim)
                .ToList();

            return ApiResponse<List<ActivityItemDto>>.Ok(sorted);
        }
    }
}
