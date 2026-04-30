using E_Commerce.Core.Features.AdminDashboard.Commands.Models;
using E_Commerce.Core.Features.AdminDashboard.Queries.Models;
using E_Commerce.Data.Status;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    public class AdminController(ISender mediator) : ControllerBase
    {
        [HttpGet("dashboard/overview")]
        public async Task<IActionResult> Overview(CancellationToken ct)
        {
            var r = await mediator.Send(new GetAdminOverviewQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("dashboard/revenue-chart")]
        public async Task<IActionResult> RevenueChart([FromQuery] int months = 12, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetRevenueChartQuery(months), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("dashboard/order-status-distribution")]
        public async Task<IActionResult> OrderStatusDistribution(CancellationToken ct)
        {
            var r = await mediator.Send(new GetOrderStatusDistributionQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("dashboard/top-sellers")]
        public async Task<IActionResult> TopSellers([FromQuery] int limit = 10, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetTopSellersQuery(limit), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("dashboard/top-products")]
        public async Task<IActionResult> TopProducts([FromQuery] int limit = 10, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetTopProductsQuery(limit), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("dashboard/activity")]
        public async Task<IActionResult> RecentActivity([FromQuery] int limit = 20, CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetRecentActivityQuery(limit), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("dashboard/financial")]
        public async Task<IActionResult> FinancialSummary(CancellationToken ct)
        {
            var r = await mediator.Send(new GetFinancialSummaryQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        // ── Users Management ────────────────────────────────────────────
      
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? search, [FromQuery] string? role,
            [FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetAllUsersQuery(search, role, isActive, page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPatch("users/{userId:guid}/manage")]
        public async Task<IActionResult> ManageUser(
            Guid userId, [FromBody] ManageUserDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new AdminManageUserCommand(userId, dto.Action, dto.Reason), ct);
            return StatusCode(r.StatusCode, r);
        }

        // ── Companies Management ────────────────────────────────────────
        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies(
            [FromQuery] string? search, [FromQuery] CompanyStatus? status,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetAllCompaniesQuery(search, status, page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPatch("companies/{companyId:guid}/manage")]
        public async Task<IActionResult> ManageCompany(
            Guid companyId, [FromBody] ManageCompanyDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new AdminManageCompanyCommand(companyId, dto.Action), ct);
            return StatusCode(r.StatusCode, r);
        }

        // ── Orders Management ───────────────────────────────────────────
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? search, [FromQuery] OrderStatus? status,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetAllOrdersQuery(search, status, from, to, page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        // ── Coupons (Admin creates platform-wide) ───────────────────────
        /// <summary>Create a platform-wide coupon (no sellerCompanyId scope).</summary>
        //[HttpPost("coupons")]
        //public async Task<IActionResult> CreateCoupon(
        //    [FromBody] CreateCouponCommand cmd, CancellationToken ct)
        //{
        //    var r = await mediator.Send(cmd, ct);
        //    return StatusCode(r.StatusCode, r);
        //}

        /// <summary>Update coupon details.</summary>
        //[HttpPut("coupons/{couponId:guid}")]
        //public async Task<IActionResult> UpdateCoupon(
        //    Guid couponId, [FromBody] UpdateCouponBody body, CancellationToken ct)
        //{
        //    var r = await mediator.Send(new UpdateCouponCommand(
        //        couponId, body.Description, body.MaxDiscountAmount, body.ExpiresAt, body.MaxUsageCount), ct);
        //    return StatusCode(r.StatusCode, r);
        //}

        /// <summary>Disable a coupon immediately.</summary>
        //[HttpDelete("coupons/{couponId:guid}")]
        //public async Task<IActionResult> DisableCoupon(Guid couponId, CancellationToken ct)
        //{
        //    var r = await mediator.Send(new DisableCouponCommand(couponId), ct);
        //    return StatusCode(r.StatusCode, r);
        //}

        //// ── Loyalty Points (Admin adjustment) ───────────────────────────
        ///// <summary>Manually adjust a buyer's loyalty points (positive or negative).</summary>
        //[HttpPost("loyalty/adjust")]
        //public async Task<IActionResult> AdjustPoints(
        //    [FromBody] AdjustPointsDto dto, CancellationToken ct)
        //{
        //    var r = await mediator.Send(new AdminAdjustPointsCommand(dto.BuyerId, dto.Delta, dto.Reason), ct);
        //    return StatusCode(r.StatusCode, r);
        //}
    }

   

    /* ════════════════════════════════════════════════════════════════════
       SELLER COUPONS CONTROLLER
       Route: /api/v1/coupons
    ════════════════════════════════════════════════════════════════════ */
    //[ApiController]
    //[Route("api/v1/coupons")]
    //[Produces("application/json")]
    //public class CouponsController(ISender mediator) : ControllerBase
    //{
    //    /// <summary>Validate a coupon code and preview the discount amount.</summary>
    //    [HttpGet("validate")]
    //    public async Task<IActionResult> Validate(
    //        [FromQuery] string code, [FromQuery] decimal orderSubTotal,
    //        [FromQuery] Guid? sellerCompanyId, CancellationToken ct)
    //    {
    //        var r = await mediator.Send(new ValidateCouponQuery(code, orderSubTotal, sellerCompanyId), ct);
    //        return StatusCode(r.StatusCode, r);
    //    }

    //    /// <summary>Create a coupon (Seller = scoped to their company, Admin = platform-wide).</summary>
    //    [HttpPost]
    //    [Authorize(Policy = "SellerOrAdmin")]
    //    public async Task<IActionResult> CreateCoupon(
    //        [FromBody] CreateCouponCommand cmd, CancellationToken ct)
    //    {
    //        var r = await mediator.Send(cmd, ct);
    //        return StatusCode(r.StatusCode, r);
    //    }

    //    /// <summary>List my coupons (Seller).</summary>
    //    [HttpGet("my")]
    //    [Authorize(Policy = "SellerOnly")]
    //    public async Task<IActionResult> GetMyCoupons(
    //        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    //    {
    //        var r = await mediator.Send(new GetMyCouponsQuery(page, pageSize), ct);
    //        return StatusCode(r.StatusCode, r);
    //    }

    //    /// <summary>Update coupon (Seller or Admin).</summary>
    //    [HttpPut("{couponId:guid}")]
    //    [Authorize(Policy = "SellerOrAdmin")]
    //    public async Task<IActionResult> UpdateCoupon(
    //        Guid couponId, [FromBody] UpdateCouponBody body, CancellationToken ct)
    //    {
    //        var r = await mediator.Send(new UpdateCouponCommand(
    //            couponId, body.Description, body.MaxDiscountAmount, body.ExpiresAt, body.MaxUsageCount), ct);
    //        return StatusCode(r.StatusCode, r);
    //    }

    //    /// <summary>Disable a coupon.</summary>
    //    [HttpDelete("{couponId:guid}")]
    //    [Authorize(Policy = "SellerOrAdmin")]
    //    public async Task<IActionResult> DisableCoupon(Guid couponId, CancellationToken ct)
    //    {
    //        var r = await mediator.Send(new DisableCouponCommand(couponId), ct);
    //        return StatusCode(r.StatusCode, r);
    //    }
    //}

    /* ════════════════════════════════════════════════════════════════════
       LOYALTY POINTS CONTROLLER
       Route: /api/v1/loyalty
    ════════════════════════════════════════════════════════════════════ */
    //[ApiController]
    //[Route("api/v1/loyalty")]
    //[Authorize]
    //[Produces("application/json")]
    //public class LoyaltyController(ISender mediator) : ControllerBase
    //{
    //    /// <summary>Get my loyalty wallet: available points, pending, lifetime.</summary>
    //    [HttpGet("wallet")]
    //    [Authorize(Policy = "BuyerOnly")]
    //    public async Task<IActionResult> GetWallet(CancellationToken ct)
    //    {
    //        var r = await mediator.Send(new GetMyLoyaltyWalletQuery(), ct);
    //        return StatusCode(r.StatusCode, r);
    //    }

    //    /// <summary>Get points transaction history (paginated).</summary>
    //    [HttpGet("history")]
    //    [Authorize(Policy = "BuyerOnly")]
    //    public async Task<IActionResult> GetHistory(
    //        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    //    {
    //        var r = await mediator.Send(new GetPointsHistoryQuery(page, pageSize), ct);
    //        return StatusCode(r.StatusCode, r);
    //    }
    //}

    /* ════════════════════════════════════════════════════════════════════
       REQUEST / BODY DTOs (for controllers)
    ════════════════════════════════════════════════════════════════════ */
    public record ManageUserDto(AdminUserAction Action, string? Reason);
    public record ManageCompanyDto(CompanyAdminAction Action, string? Reason);
    public record AdjustPointsDto(Guid BuyerId, int Delta, string Reason);
    public record UpdateOptionBody(string Name, int DisplayOrder = 0);
    public record AddValueBody(string Value, string? DisplayLabel = null, int DisplayOrder = 0);
    public record UpdateVariantBody(string SKU, decimal Price, int StockQuantity, bool IsActive, string? ImageUrl = null);
    public record PatchStockBody(int Delta);
    public record UpdatePriceTierBody(int MinQuantity, decimal UnitPrice, int? MaxQuantity = null);
    public record UpdateCouponBody(string Description, decimal? MaxDiscountAmount, DateTime? ExpiresAt, int? MaxUsageCount);

}

