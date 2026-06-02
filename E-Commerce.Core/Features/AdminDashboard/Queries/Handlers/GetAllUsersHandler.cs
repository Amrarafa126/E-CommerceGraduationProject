using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{
    public class GetAllUsersHandler(
    AppDBContext db,
    ICurrentUserService cu)
    : IRequestHandler<GetAllUsersQuery, ApiResponse<PaginatedResult<AdminUserDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<AdminUserDto>>> Handle(
            GetAllUsersQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var q = db.Users.AsNoTracking()
                .Include(u => u.OwnedCompany)
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.ToLower();
                q = q.Where(u => u.Email!.Contains(s) || u.FirstName.Contains(s) || u.LastName.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(req.Role))
            {
                if (Enum.TryParse<UserRole>(req.Role, true, out var roleEnum))
                    q = q.Where(u => u.Role == roleEnum);
            }

            if (req.IsActive.HasValue)
                q = q.Where(u => u.IsActive == req.IsActive.Value);

            var total = await q.CountAsync(ct);
            var users = await q
                .OrderByDescending(u => u.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(ct);

            var dtos = users.Select(u => new AdminUserDto(
                u.Id, u.Email ?? "", u.FirstName, u.LastName, u.FullName,
                u.Role.ToString(), u.Role.ToString(),
                u.IsActive, u.EmailConfirmed, u.LockoutEnd,
                u.OwnedCompanyId, u.OwnedCompany?.CompanyName,
                u.CreatedAt, u.UpdatedAt)).ToList();

            return ApiResponse<PaginatedResult<AdminUserDto>>.Ok(
                PaginatedResult<AdminUserDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
