using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Core.Wrappers;
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

    public class GetAllOrdersHandler(AppDBContext db, ICurrentUserService cu)
        : IRequestHandler<GetAllOrdersQuery, ApiResponse<PaginatedResult<AdminOrderDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<AdminOrderDto>>> Handle(
            GetAllOrdersQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var q = db.orders.AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.SellerCompany)
                .Include(o => o.Payment)
                .Where(o => !o.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.ToLower();
                q = q.Where(o => o.Buyer.Email!.ToLower().Contains(s) || o.SellerCompany.CompanyName.ToLower().Contains(s));
            }

            if (req.Status.HasValue) q = q.Where(o => o.Status == req.Status.Value);
            if (req.From.HasValue) q = q.Where(o => o.CreatedAt >= req.From.Value);
            if (req.To.HasValue) q = q.Where(o => o.CreatedAt <= req.To.Value);

            var total = await q.CountAsync(ct);
            var orders = await q
                .OrderByDescending(o => o.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(ct);

            var dtos = orders.Select(o => new AdminOrderDto(
                o.Id, o.Buyer.FullName, o.Buyer.Email ?? "",
                o.SellerCompany.CompanyName, o.Status.ToString(),
                o.SubTotal, o.ShippingCost, o.TaxAmount,
                o.TotalAmount, o.Currency,
                o.Payment?.Status.ToString(),
                o.CreatedAt)).ToList();

            return ApiResponse<PaginatedResult<AdminOrderDto>>.Ok(
                PaginatedResult<AdminOrderDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }

}
