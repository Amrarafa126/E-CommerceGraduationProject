using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetAllCompaniesHandler(AppDBContext db, ICurrentUserService cu)
     : IRequestHandler<GetAllCompaniesQuery, ApiResponse<PaginatedResult<AdminCompanyDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<AdminCompanyDto>>> Handle(
            GetAllCompaniesQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("مسموح فقط للمسؤول.");

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
                q = q.Where(c => (int)c.Status == req.Status.Value + 1);

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

            var orderCounts = await db.orderSubOrders
                .Where(s => companies.Select(c => c.Id).Contains(s.SellerCompanyId) && !s.IsDeleted)
                .GroupBy(s => s.SellerCompanyId)
                .Select(g => new { CompanyId = g.Key, Count = g.Count(), Revenue = g.Sum(s => s.TotalAmount) })
                .ToDictionaryAsync(x => x.CompanyId, x => new { x.Count, x.Revenue }, ct);

            var dtos = companies.Select(c => new AdminCompanyDto(
                c.Id, c.CompanyName, c.Description, (int)c.Status - 1,
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
