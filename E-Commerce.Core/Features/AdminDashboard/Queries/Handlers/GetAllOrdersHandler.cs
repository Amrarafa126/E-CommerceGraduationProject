using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetAllOrdersHandler(AppDBContext db, ICurrentUserService cu)
        : IRequestHandler<GetAllOrdersQuery, ApiResponse<PaginatedResult<AdminOrderDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<AdminOrderDto>>> Handle(
            GetAllOrdersQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("مسموح فقط للمسؤول.");

            var q = db.orders.AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.SellerCompany)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Payment)
                .Where(o => !o.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.ToLower();
                q = q.Where(o => o.Buyer.Email!.ToLower().Contains(s)
                    || o.SubOrders.Any(sub => sub.SellerCompany.CompanyName.ToLower().Contains(s)));
            }

            if (req.Status.HasValue)
                q = q.Where(o => o.SubOrders.Any(sub => (int)sub.Status == req.Status.Value + 1));
            if (req.From.HasValue) q = q.Where(o => o.CreatedAt >= req.From.Value);
            if (req.To.HasValue) q = q.Where(o => o.CreatedAt <= req.To.Value);

            var total = await q.CountAsync(ct);
            var orders = await q
                .OrderByDescending(o => o.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(ct);

            var dtos = orders.Select(o => {
                var sub = o.SubOrders.FirstOrDefault();
                return new AdminOrderDto(
                    o.Id, o.Buyer.FullName, o.Buyer.Email ?? "",
                    sub?.SellerCompany?.CompanyName ?? "",
                    sub != null ? (int)sub.Status - 1 : 0,
                    sub?.SubTotal ?? 0, sub?.ShippingCost ?? 0, sub?.TaxAmount ?? 0,
                    sub?.TotalAmount ?? 0, o.Currency,
                    sub?.Payment?.Status.ToString(),
                    o.CreatedAt);
            }).ToList();

            return ApiResponse<PaginatedResult<AdminOrderDto>>.Ok(
                PaginatedResult<AdminOrderDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
