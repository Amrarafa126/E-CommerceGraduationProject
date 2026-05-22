using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard
{
    public record AdminOverviewDto(
    // Users
    int TotalUsers, int TotalSellers, int TotalBuyers,
    int NewUsersToday, int NewUsersThisMonth,
    // Companies
    int TotalCompanies, int ActiveCompanies, int PendingCompanies, int NewCompaniesThisMonth,
    // Products
    int TotalProducts, int ActiveProducts, int DraftProducts,
    // Orders
    int TotalOrders, int OrdersToday, int OrdersThisMonth, int PendingOrders,
    // Revenue
    decimal RevenueTotal, decimal RevenueToday, decimal RevenueThisMonth,
    decimal RevenueLastMonth, decimal RevenueThisYear, double RevenueMonthGrowthPct,
    // Engagement
    int TotalReviews, double AvgRating, int TotalRfqs, int PendingRfqs,
    // Meta
    DateTime GeneratedAt);

    public record RevenueChartPointDto(
        string MonthKey, string MonthLabel,
        decimal Revenue, int Orders, int NewUsers);

    public record OrderStatusCountDto(int Status, int Count, decimal Revenue);

    public record AdminUserDto(
        Guid Id, string Email, string FirstName, string LastName, string FullName,
        string DomainRole, string IdentityRoles,
        bool IsActive, bool IsEmailVerified, DateTimeOffset? LockoutEnd,
        Guid? OwnedCompanyId, string? OwnedCompanyName,
        DateTime CreatedAt, DateTime? UpdatedAt);

    public record AdminCompanyDto(
        Guid Id, string Name, string Description, int Status,
        string OwnerName, string OwnerEmail,
        string? Country, int? YearEstablished, int EmployeesCount,
        decimal AvailableBalance, decimal PendingBalance,
        int ProductCount, int OrderCount, decimal TotalRevenue,
        bool IsActive, DateTime CreatedAt);

    public record AdminOrderDto(
        Guid Id, string BuyerName, string BuyerEmail,
        string SellerCompanyName, int Status,
        decimal SubTotal, decimal ShippingCost, decimal TaxAmount,
         decimal TotalAmount, string Currency,
        string? PaymentStatus, 
        DateTime CreatedAt);

    public record TopSellerDto(
        int Rank, Guid CompanyId, string CompanyName,
        decimal Revenue, int OrderCount, int ProductCount);

    public record TopProductDto(
        int Rank, Guid ProductId, string ProductName, string? ImageUrl,
        string CompanyName, int TotalQuantitySold, decimal TotalRevenue, int OrderCount);

    public record ActivityItemDto(
        string Type, string Title, string Description,
        string Icon, DateTime OccurredAt,
        Guid? OrderId, Guid? UserId, Guid? CompanyId);

    public record FinancialSummaryDto(
        decimal TotalGmv, decimal RefundedAmount,
        decimal TotalAvailableBalance, decimal TotalPendingBalance,
        decimal TotalPaidOut, decimal PendingPayouts);

}
