using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductVideo : BaseEntity
    {
        public string Url { get; private set; } = string.Empty;
        public string? Title { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public int DisplayOrder { get; private set; }
        public int? DurationSeconds { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        private ProductVideo() { }

        public static ProductVideo Create(Guid productId, string url,
            string? title = null, string? thumbnailUrl = null,
            int displayOrder = 0, int? durationSeconds = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Video URL cannot be empty.");

            return new ProductVideo
            {
                ProductId = productId,
                Url = url.Trim(),
                Title = title?.Trim(),
                ThumbnailUrl = thumbnailUrl?.Trim(),
                DisplayOrder = displayOrder,
                DurationSeconds = durationSeconds
            };
        }

        public void Update(string url, string? title = null, string? thumbnailUrl = null,
            int displayOrder = 0, int? durationSeconds = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Video URL cannot be empty.");

            Url = url.Trim();
            Title = title?.Trim();
            ThumbnailUrl = thumbnailUrl?.Trim();
            DisplayOrder = displayOrder;
            DurationSeconds = durationSeconds;
            MarkAsUpdated();
        }
    }
}
