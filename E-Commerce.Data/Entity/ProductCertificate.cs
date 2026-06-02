using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class ProductCertificate : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Url { get; private set; } = string.Empty;
        public string OriginalFileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long FileSizeBytes { get; private set; }
        public string? IssuedBy { get; private set; }
        public DateTime? ValidUntil { get; private set; }
        public int DisplayOrder { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        private ProductCertificate() { }

        public static ProductCertificate Create(Guid productId, string name, string url,
            string originalFileName, string contentType, long fileSizeBytes,
            string? issuedBy = null, DateTime? validUntil = null, int displayOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Certificate name cannot be empty.");
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Certificate URL cannot be empty.");

            return new ProductCertificate
            {
                ProductId = productId,
                Name = name.Trim(),
                Url = url,
                OriginalFileName = originalFileName,
                ContentType = contentType,
                FileSizeBytes = fileSizeBytes,
                IssuedBy = issuedBy?.Trim(),
                ValidUntil = validUntil,
                DisplayOrder = displayOrder
            };
        }

        public void Update(string name, string? issuedBy = null, DateTime? validUntil = null, int displayOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Certificate name cannot be empty.");

            Name = name.Trim();
            IssuedBy = issuedBy?.Trim();
            ValidUntil = validUntil;
            DisplayOrder = displayOrder;
            MarkAsUpdated();
        }
    }
}
