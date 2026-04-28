using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ReviewImage : BaseEntity
    {
        public Guid ReviewId { get; private set; }
        public ProductReview Review { get; private set; } = null!;
        public string Url { get; private set; } = string.Empty;
        public int DisplayOrder { get; private set; }

        private ReviewImage() { }

        public static ReviewImage Create(Guid reviewId, string url, int order = 0) => new()
        {
            ReviewId = reviewId,
            Url = url,
            DisplayOrder = order
        };
    }
}
