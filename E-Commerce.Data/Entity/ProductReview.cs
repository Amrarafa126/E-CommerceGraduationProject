using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class ProductReview : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;
        public int Rating { get; private set; }           // 1–5
        public string Title { get; private set; } = string.Empty;
        public string Comment { get; private set; } = string.Empty;
        public bool IsVerifiedPurchase { get; private set; }
        public string? SupplierReply { get; private set; }
        public DateTime? RepliedAt { get; private set; }
        public bool IsVisible { get; private set; } = true;
        public bool IsApproved { get; private set; } = true;
        public int HelpfulCount { get; private set; }
        public ICollection<ReviewImage> Images { get; private set; } = new List<ReviewImage>();
        public ICollection<ReviewHelpfulVote> HelpfulVotes { get; private set; } = new List<ReviewHelpfulVote>();

        private ProductReview() { }

        public static ProductReview Create(Guid productId, Guid buyerId,
            int rating, string title, string comment, bool isVerified = false)
        {
            if (rating is < 1 or > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");
            return new ProductReview
            {
                ProductId = productId,
                BuyerId = buyerId,
                Rating = rating,
                Title = title,
                Comment = comment,
                IsVerifiedPurchase = isVerified
            };
        }

        public void Update(int rating, string title, string comment)
        {
            if (rating is < 1 or > 5) throw new ArgumentException("Rating must be between 1 and 5.");
            Rating = rating;
            Title = title;
            Comment = comment;
            MarkAsUpdated();
        }

        public void AddSupplierReply(string reply)
        {
            SupplierReply = reply;
            RepliedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void SetVisibility(bool visible)
        {
            IsVisible = visible;
            MarkAsUpdated();
        }

        public void SetApproved(bool approved)
        {
            IsApproved = approved;
            MarkAsUpdated();
        }

        public void IncrementHelpful() { HelpfulCount++; MarkAsUpdated(); }
        public void DecrementHelpful() { if (HelpfulCount > 0) HelpfulCount--; MarkAsUpdated(); }
    }
}
