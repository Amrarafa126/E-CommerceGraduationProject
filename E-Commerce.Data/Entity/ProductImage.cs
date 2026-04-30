using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductImage : BaseEntity
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public string Url { get; private set; } = string.Empty;
        public string OriginalFileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long FileSizeBytes { get; private set; }
        public string? AltText { get; private set; }
        public int DisplayOrder { get; private set; } = 0;
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        private ProductImage() { }

        public static ProductImage Create(Guid productId, string url, string originalFileName,
            string contentType, long fileSizeBytes, int displayOrder, string? altText = null)
        {
            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new ArgumentException($"File type '{ext}' is not allowed. Use: jpg, png, webp.");

            if (fileSizeBytes > MaxFileSizeBytes)
                throw new ArgumentException($"File size exceeds {MaxFileSizeBytes / 1024 / 1024}MB limit.");

            return new ProductImage
            {
                ProductId = productId,
                Url = url,
                OriginalFileName = originalFileName,
                ContentType = contentType,
                FileSizeBytes = fileSizeBytes,
                DisplayOrder = displayOrder,
                AltText = altText
            };


        }

        public void UpdateMetadata(string? altText, int displayOrder)
        {
            if (displayOrder < 0)
                throw new ArgumentException("Display order cannot be negative");
            AltText = altText;
            DisplayOrder = displayOrder;
        }

        public void UpdateFileDetails(string url, string fileName, string contentType, long size)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL is required");

            if (size <= 0)
                throw new ArgumentException("Invalid file size");
            Url = url;
            OriginalFileName = fileName;
            ContentType = contentType;
            FileSizeBytes = size;
        }

    }
}
