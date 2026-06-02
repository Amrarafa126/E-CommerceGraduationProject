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
        public string OriginalFileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long FileSizeBytes { get; private set; }
        public int DisplayOrder { get; private set; }

        private ReviewImage() { }

        public static ReviewImage Create(Guid reviewId, string url, string originalFileName,
            string contentType, long fileSizeBytes, int order = 0) => new()
        {
            ReviewId = reviewId,
            Url = url,
            OriginalFileName = originalFileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            DisplayOrder = order
        };
    }
}
