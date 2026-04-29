using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
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
    public class GetTopProductsHandler(AppDBContext db, ICurrentUserService cu)
     : IRequestHandler<GetTopProductsQuery, ApiResponse<List<TopProductDto>>>
    {
        public async Task<ApiResponse<List<TopProductDto>>> Handle(
            GetTopProductsQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var result = await db.orderItems
                .Where(i => !i.Order.IsDeleted)
                .GroupBy(i => new { i.ProductId, i.ProductName, i.Product.MainImageUrl, i.Product.Company.CompanyName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    g.Key.MainImageUrl,
                    CompanyName = g.Key.CompanyName,
                    TotalQty = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.TotalPrice),
                    OrderCount = g.Count(),
                })
                .OrderByDescending(x => x.TotalQty)
                .Take(Math.Clamp(req.Limit, 1, 50))
                .ToListAsync(ct);

            var dtos = result.Select((r, i) => new TopProductDto(
                i + 1, r.ProductId, r.ProductName, r.MainImageUrl,
                r.CompanyName, r.TotalQty, r.TotalRevenue, r.OrderCount)).ToList();

            return ApiResponse<List<TopProductDto>>.Ok(dtos);
        }
    }

}
