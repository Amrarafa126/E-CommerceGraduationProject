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
    public class GetAllCompaniesHandler(AppDBContext db, ICurrentUserService cu)
     : IRequestHandler<GetAllCompaniesQuery, ApiResponse<PaginatedResult<AdminCompanyDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<AdminCompanyDto>>> Handle(
            GetAllCompaniesQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var q = db.companies.AsNoTracking()
                .Include(c => c.Owner)
                .Include(c => c.Wallet)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.ToLower();
                q = q.Where(c => c.CompanyName.ToLower().Contains(s) || c.Owner.Email!.ToLower().Contains(s));
            }

            if (req.Status.HasValue)
                q = q.Where(c => c.Status == req.Status.Value);

            var total = await q.CountAsync(ct);
            var companies = await q
                .OrderByDescending(c => c.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(ct);

            var productCounts = await db.products
                .Where(p => companies.Select(c => c.Id).Contains(p.CompanyId) && !p.IsDeleted)
                .GroupBy(p => p.CompanyId)
                .Select(g => new { CompanyId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CompanyId, x => x.Count, ct);

            var orderCounts = await db.orders
                .Where(o => companies.Select(c => c.Id).Contains(o.SellerCompanyId) && !o.IsDeleted)
                .GroupBy(o => o.SellerCompanyId)
                .Select(g => new { CompanyId = g.Key, Count = g.Count(), Revenue = g.Sum(o => o.TotalAmount) })
                .ToDictionaryAsync(x => x.CompanyId, x => new { x.Count, x.Revenue }, ct);

            var dtos = companies.Select(c => new AdminCompanyDto(
                c.Id, c.CompanyName, c.Description, c.Status.ToString(),
                c.Owner.FullName, c.Owner.Email ?? "",
                c.Address?.Country, c.YearEstablished, c.EmployeesCount,
                c.Wallet?.AvailableBalance ?? 0, c.Wallet?.PendingBalance ?? 0,
                productCounts.GetValueOrDefault(c.Id),
                orderCounts.TryGetValue(c.Id, out var ord) ? ord.Count : 0,
                orderCounts.TryGetValue(c.Id, out var rev) ? rev.Revenue : 0,
                c.IsActive, c.CreatedAt)).ToList();

            return ApiResponse<PaginatedResult<AdminCompanyDto>>.Ok(
                PaginatedResult<AdminCompanyDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
