using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
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

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Handlers
{

    public class GetFinancialSummaryHandler(AppDBContext db, ICurrentUserService cu)
        : IRequestHandler<GetFinancialSummaryQuery, ApiResponse<FinancialSummaryDto>>
    {
        public async Task<ApiResponse<FinancialSummaryDto>> Handle(
            GetFinancialSummaryQuery req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var totalAvailableBalance = await db.Wallets
                .SumAsync(w => (decimal?)w.AvailableBalance, ct) ?? 0;

            var totalPendingBalance = await db.Wallets
                .SumAsync(w => (decimal?)w.PendingBalance, ct) ?? 0;

            var totalPaidOut = await db.payouts
                .Where(p => p.Status == PayoutStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0;

            var pendingPayouts = await db.payouts
                .Where(p => p.Status == PayoutStatus.Pending)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0;

            var totalGmv = await db.orders
                .Where(o => !o.IsDeleted && o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Refunded)
                .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0;

            var refundedAmount = await db.orders
                .Where(o => !o.IsDeleted && o.Status == OrderStatus.Refunded)
                .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0;

            //var couponDiscountsTotal = await db.orders
            //    .Where(o => !o.IsDeleted && o.CouponDiscount > 0)
            //    .SumAsync(o => (decimal?)o.CouponDiscount, ct) ?? 0;

            //var pointsDiscountsTotal = await db.orders
            //    .Where(o => !o.IsDeleted && o.PointsDiscount > 0)
            //    .SumAsync(o => (decimal?)o.PointsDiscount, ct) ?? 0;

            return ApiResponse<FinancialSummaryDto>.Ok(new FinancialSummaryDto(
                totalGmv, refundedAmount, totalAvailableBalance, totalPendingBalance,
                totalPaidOut, pendingPayouts));
        }
    }

}
