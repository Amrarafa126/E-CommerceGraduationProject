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

        private Product() { }

        public static Product Create(string name, string description, Guid companyId,
            Guid categoryId, int moq, decimal basePrice , string currency)
        {
            if (moq <= 0) throw new ArgumentException("MOQ must be > 0.");
           if (basePrice < 0) throw new ArgumentException("Price cannot be negative.");

            return new Product
            {
                Name = name,
                Description = description,
                CompanyId = companyId,
                CategoryId = categoryId,
                MinimumOrderQuantity = moq,
                BasePrice = basePrice,
                Currency = currency,
         
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
               .Where(t => quantity >= t.MinQuantity)
               .OrderByDescending(t => t.MinQuantity)
               .FirstOrDefault();
            return tier?.UnitPrice ?? BasePrice;
        }

        public void UpdateRatingStats(double avgRating, int reviewCount)
        {
            AverageRating = avgRating;
            ReviewCount = reviewCount;
            MarkAsUpdated();
        }

    }
}
