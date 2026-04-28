using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.DashboardComapny
{
    public record CompanyDashboardDto(OverviewDto Overview, FinancialDto Financial, ReviewSummaryDto ReviewSummary, ChatSummaryDto ChatSummary, RfqSummaryDto RfqSummary, List<TopProductDto> TopProducts, List<MonthlyRevenueDto> MonthlyRevenue, Dictionary<string, int> OrderStatusDistribution);
    public record OverviewDto(int TotalProducts, int ActiveProducts, int TotalOrders, int PendingOrders, int ProcessingOrders);
    public record FinancialDto(decimal TotalRevenue, decimal MonthlyRevenue, decimal PendingBalance, decimal AvailableBalance);
    public record ReviewSummaryDto(double AverageRating, int TotalReviews);
    public record ChatSummaryDto(int UnreadMessages, int ActiveConversations);
    public record RfqSummaryDto(int PendingRfqs, int TotalRfqs);
    public record TopProductDto(Guid ProductId, string ProductName, string? MainImageUrl, int OrderCount, decimal Revenue, double AverageRating);
    public record MonthlyRevenueDto(string Month, int Year, decimal Revenue, int OrderCount);

}
