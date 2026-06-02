using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class ReviewHelpfulVote : BaseEntity
    {
        public Guid ReviewId { get; private set; }
        public ProductReview Review { get; private set; } = null!;
        public Guid ReviewerId { get; private set; }
        public User Reviewer { get; private set; } = null!;

        private ReviewHelpfulVote() { }

        public static ReviewHelpfulVote Create(Guid reviewId, Guid reviewerId)
            => new()
            {
                ReviewId = reviewId,
                ReviewerId = reviewerId
            };
    }
}
