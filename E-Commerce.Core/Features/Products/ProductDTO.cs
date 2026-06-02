using E_Commerce.Core.Features.ProductImages;
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductPriceTiers;
using E_Commerce.Core.Features.ProductVariants;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products
{
    // ─── New DTOs for product sub-entities ───
    public record ProductSpecificationDto(Guid Id, string Name, string Value,
        string? GroupName, int DisplayOrder, bool IsHighlight);

    public record ProductCertificateDto(Guid Id, string Name, string Url,
        string OriginalFileName, string ContentType, long FileSizeBytes,
        string? IssuedBy, DateTime? ValidUntil, int DisplayOrder);

    public record ProductVideoDto(Guid Id, string Url, string? Title,
        string? ThumbnailUrl, int DisplayOrder, int? DurationSeconds);

    public record ProductTagDto(Guid Id, string Tag);

    public record ProductDto(Guid Id, string Name, string Description,
        string? MainImageUrl, decimal BasePrice, string Currency, int MinimumOrderQuantity, int StockQuantity, int Status,
        Guid CompanyId, string? CompanyName, Guid CategoryId, string? CategoryName, double AverageRating,
        int ReviewCount, List<ProductImageDto> Images, List<ProductOptionDto> Options, List<ProductVariantDto> Variants,
        List<PriceTierDto> PriceTiers,
        // B2B fields
        string? Brand, string? ModelNumber, string? OriginCountry, int UnitOfMeasure,
        int LeadTimeDays, int? LeadTimeDaysSample, string? SupplyAbility, string? TradeTerms,
        string? PortOfLoading, string? PaymentTerms, string? PackagingDetails,
        bool SampleAvailable, decimal? SamplePrice, int? SampleMoq,
        string? MetaTitle, string? MetaDescription, string? Slug,
        List<ProductSpecificationDto> Specifications, List<ProductCertificateDto> Certificates,
        List<ProductVideoDto> Videos, List<ProductTagDto> Tags,
        DateTime CreatedAt, DateTime? UpdatedAt);

    public record CreateProductDto(string Name, string Description, Guid CategoryId,
        int MinimumOrderQuantity, decimal BasePrice, string Currency,
        int StockQuantity,
        // B2B optional fields
        string? Brand, string? ModelNumber, string? OriginCountry, int? UnitOfMeasure,
        int? LeadTimeDays, int? LeadTimeDaysSample, string? SupplyAbility, string? TradeTerms,
        string? PortOfLoading, string? PaymentTerms, string? PackagingDetails,
        bool? SampleAvailable, decimal? SamplePrice, int? SampleMoq,
        string? MetaTitle, string? MetaDescription, string? Slug);

    public class ProductSummaryDto
    {
        public ProductSummaryDto() { }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? MainImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public int StockQuantity { get; set; }
        public int Status { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string? Brand { get; set; }
        public int LeadTimeDays { get; set; }
        public bool SampleAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record UpdateProductDto(string Name, string Description, Guid CategoryId, int MinimumOrderQuantity,
        decimal BasePrice, int StockQuantity,
        // B2B optional fields
        string? Brand, string? ModelNumber, string? OriginCountry, int? UnitOfMeasure,
        int? LeadTimeDays, int? LeadTimeDaysSample, string? SupplyAbility, string? TradeTerms,
        string? PortOfLoading, string? PaymentTerms, string? PackagingDetails,
        bool? SampleAvailable, decimal? SamplePrice, int? SampleMoq,
        string? MetaTitle, string? MetaDescription, string? Slug);

    // ─── Sub-entity command DTOs ───
    public record AddProductSpecificationDto(string Name, string Value,
        string? GroupName, int DisplayOrder, bool IsHighlight);

    public record UpdateProductSpecificationDto(string Name, string Value,
        string? GroupName, int DisplayOrder, bool IsHighlight);

    public record AddProductCertificateDto(string Name, string Url,
        string OriginalFileName, string ContentType, long FileSizeBytes,
        string? IssuedBy, DateTime? ValidUntil);

    public record AddProductVideoDto(string Url, string? Title,
        string? ThumbnailUrl, int? DurationSeconds);

    public record AddProductTagDto(string Tag);

    internal static class ProductMapper
    {
        internal static int MapProductStatus(ProductStatus status) => status switch
        {
            ProductStatus.Draft => 0,
            ProductStatus.Active => 1,
            ProductStatus.Inactive => 3,
            _ => 0
        };

        internal static ProductDto Map(Product p) => new(
            p.Id, p.Name, p.Description, p.MainImageUrl,
            p.BasePrice, p.Currency, p.MinimumOrderQuantity, p.StockQuantity, (int)p.Status,
            p.CompanyId, p.Company?.CompanyName, p.CategoryId, p.Category?.Name,
            p.AverageRating, p.ReviewCount,
            p.Images.Where(i => !i.IsDeleted).OrderBy(i => i.DisplayOrder)
                .Select(i => new ProductImageDto(
                    i.Id, i.Url, i.OriginalFileName, i.FileSizeBytes, i.AltText, i.DisplayOrder))
                .ToList(),
            p.ProductOptions.OrderBy(o => o.DisplayOrder)
                .Select(o => new ProductOptionDto(
                    o.Id, o.Name, o.DisplayOrder,
                    o.Values.OrderBy(v => v.DisplayOrder)
                        .Select(v => new ProductOptionValueDto(v.Id, v.Value, v.DisplayOrder))
                        .ToList()))
                .ToList(),
            p.productVariants.Select(v => new ProductVariantDto(
                v.Id, v.SKU, v.Price, v.StockQuantity, v.IsActive,
                v.Barcode, v.ImageUrl,
                v.OptionValues.Select(ov => new VariantOptionValueDto(
                    ov.ProductOptionValue.Id,
                    ov.ProductOptionValue.Value,
                    ov.ProductOptionValue.ProductOption.Name)).ToList()))
                .ToList(),
            p.PriceTiers.OrderBy(t => t.MinQuantity)
                .Select(t => new PriceTierDto(t.Id, t.MinQuantity, t.MaxQuantity, t.UnitPrice))
                .ToList(),
            // B2B fields
            p.Brand, p.ModelNumber, p.OriginCountry, (int)p.UnitOfMeasure,
            p.LeadTimeDays, p.LeadTimeDaysSample, p.SupplyAbility, p.TradeTerms,
            p.PortOfLoading, p.PaymentTerms, p.PackagingDetails,
            p.SampleAvailable, p.SamplePrice, p.SampleMoq,
            p.MetaTitle, p.MetaDescription, p.Slug,
            p.Specifications.Where(s => !s.IsDeleted).OrderBy(s => s.DisplayOrder)
                .Select(s => new ProductSpecificationDto(s.Id, s.Name, s.Value, s.GroupName, s.DisplayOrder, s.IsHighlight))
                .ToList(),
            p.Certificates.Where(c => !c.IsDeleted).OrderBy(c => c.DisplayOrder)
                .Select(c => new ProductCertificateDto(c.Id, c.Name, c.Url, c.OriginalFileName, c.ContentType, c.FileSizeBytes,
                    c.IssuedBy, c.ValidUntil, c.DisplayOrder))
                .ToList(),
            p.Videos.Where(v => !v.IsDeleted).OrderBy(v => v.DisplayOrder)
                .Select(v => new ProductVideoDto(v.Id, v.Url, v.Title, v.ThumbnailUrl, v.DisplayOrder, v.DurationSeconds))
                .ToList(),
            p.Tags.Where(t => !t.IsDeleted)
                .Select(t => new ProductTagDto(t.Id, t.Tag))
                .ToList(),
            p.CreatedAt, p.UpdatedAt);
    }
}
