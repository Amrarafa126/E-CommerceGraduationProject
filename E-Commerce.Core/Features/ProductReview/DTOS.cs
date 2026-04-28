using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview
{
    public record ProductReviewDto(Guid Id, Guid ProductId, string ProductName, Guid BuyerId, string BuyerName, int Rating, string Title, string Comment, bool IsVerifiedPurchase, string? SupplierReply, DateTime? RepliedAt, List<string> ImageUrls, DateTime CreatedAt, DateTime? UpdatedAt);
    public record ReviewStatsDto(double AverageRating, int TotalReviews, Dictionary<int, int> RatingDistribution);
    public record CreateReviewDto(Guid ProductId, int Rating, string Title, string Comment);
    public record UpdateReviewDto(int Rating, string Title, string Comment);
    public record SupplierReplyDto(string Reply);
}
