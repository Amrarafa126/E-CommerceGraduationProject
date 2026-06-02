using E_Commerce.Data.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Product : BaseEntity
    {
      
        private const int MaxImages = 6;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? MainImageUrl { get; private set; }
        public string? Currency { get; set; } = "EGP";  
        public decimal BasePrice { get; private set; }
        public double AverageRating { get; private set; }
        public int ReviewCount { get; private set; }
        public int MinimumOrderQuantity { get; set; }
        public int StockQuantity { get; private set; }
        public ProductStatus Status { get; private set; } = ProductStatus.Draft;

        // ─── B2B Fields ───
        public string? Brand { get; set; }
        public string? ModelNumber { get; set; }
        public string? OriginCountry { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
        public int LeadTimeDays { get; set; }
        public int? LeadTimeDaysSample { get; set; }
        public string? SupplyAbility { get; set; }
        public string? TradeTerms { get; set; }
        public string? PortOfLoading { get; set; }
        public string? PaymentTerms { get; set; }
        public string? PackagingDetails { get; set; }
        public bool SampleAvailable { get; set; }
        public decimal? SamplePrice { get; set; }
        public int? SampleMoq { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug { get; set; }

        public Guid CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductPriceTier> PriceTiers { get; set; } = new List<ProductPriceTier>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public ICollection<ProductVariant> productVariants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

        // New collections
        public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public ICollection<ProductCertificate> Certificates { get; set; } = new List<ProductCertificate>();
        public ICollection<ProductVideo> Videos { get; set; } = new List<ProductVideo>();
        public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();

        private Product() { }

        public static Product Create(string name, string description, Guid companyId,
            Guid categoryId, int moq, decimal basePrice, string currency, int stockQuantity = 0)
        {
            if (moq <= 0) throw new ArgumentException("MOQ must be > 0.");
            if (basePrice < 0) throw new ArgumentException("Price cannot be negative.");
            if (stockQuantity < 0) throw new ArgumentException("Stock quantity cannot be negative.");

            return new Product
            {
                Name = name,
                Description = description,
                CompanyId = companyId,
                CategoryId = categoryId,
                MinimumOrderQuantity = moq,
                BasePrice = basePrice,
                Currency = currency,
                StockQuantity = stockQuantity
            };
        }
        public void Update(string name, string description, Guid categoryId,
            int moq, decimal basePrice, int stockQuantity)
        {
            Name = name; Description = description; CategoryId = categoryId;
            MinimumOrderQuantity = moq; BasePrice = basePrice;
            StockQuantity = stockQuantity;
            MarkAsUpdated();

        }

        public void SetStock(int quantity)
        {
            StockQuantity = quantity;
            MarkAsUpdated();
        }

        public void AdjustStock(int delta)
        {
            StockQuantity += delta;
            if (StockQuantity < 0) StockQuantity = 0;
            MarkAsUpdated();
        }

        public void AddImage(ProductImage image)
        {
            var activeImages = Images.Where(i => !i.IsDeleted).ToList();
            if (activeImages.Count >= MaxImages)
                throw new InvalidOperationException(
                    $"A product can have at most {MaxImages} images.");
            Images.Add(image);
            if (MainImageUrl == null) MainImageUrl = image.Url;
            MarkAsUpdated();
        }

        public void RemoveImage(Guid imageId)
        {
            var image = Images.FirstOrDefault(i => i.Id == imageId)
                ?? throw new ArgumentException("Image not found.");
            image.SoftDelete();
            if (MainImageUrl == image.Url)
                MainImageUrl = Images.FirstOrDefault(i => !i.IsDeleted)?.Url;
            MarkAsUpdated();
        }

        public void SetMainImage(string url) { MainImageUrl = url; MarkAsUpdated(); }
        [NotMapped]
        public int ActiveImageCount => Images.Count(i => !i.IsDeleted);

        public void Publish()
        {
            if (!Images.Any(i => !i.IsDeleted))
                throw new InvalidOperationException("Product needs at least one image.");
            Status = ProductStatus.Active;
            MarkAsUpdated();
        }

        public void Deactivate() { Status = ProductStatus.Inactive; MarkAsUpdated(); }

        public decimal GetPriceForQuantity(int quantity)
        {
            var tier = PriceTiers
               .Where(t => quantity >= t.MinQuantity && (!t.MaxQuantity.HasValue || quantity <= t.MaxQuantity.Value))
               .OrderByDescending(t => t.MinQuantity)
               .FirstOrDefault();
            return tier?.UnitPrice ?? BasePrice;
        }

        public bool HasOverlappingTiers()
        {
            var activeTiers = PriceTiers.OrderBy(t => t.MinQuantity).ToList();
            for (int i = 0; i < activeTiers.Count - 1; i++)
            {
                var current = activeTiers[i];
                var next = activeTiers[i + 1];
                var currentMax = current.MaxQuantity ?? int.MaxValue;
                if (currentMax >= next.MinQuantity)
                    return true;
            }
            return false;
        }

        public void UpdateRatingStats(double avgRating, int reviewCount)
        {
            AverageRating = avgRating;
            ReviewCount = reviewCount;
            MarkAsUpdated();
        }

    }
}
