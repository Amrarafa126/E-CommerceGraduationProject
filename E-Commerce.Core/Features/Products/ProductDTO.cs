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

    public record ProductDto(Guid Id, string Name, string Description,
        string? MainImageUrl, decimal BasePrice, string Currency, int MinimumOrderQuantity, int Status,
        Guid CompanyId, string? CompanyName, Guid CategoryId, string? CategoryName, double AverageRating,
        int ReviewCount, List<ProductImageDto> Images, List<ProductOptionDto> Options, List<ProductVariantDto> Variants,
        List<PriceTierDto> PriceTiers, DateTime CreatedAt, DateTime? UpdatedAt);

    //public class ProductDto
    //{
    //    public Guid Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string? MainImageUrl { get; set; }
    //    public decimal BasePrice { get; set; }
    //    public string Currency { get; set; }
    //    public int MinimumOrderQuantity { get; set; }
    //    public string Status { get; set; }
    //    public Guid CompanyId { get; set; }
    //    public string? CompanyName { get; set; }
    //    public Guid CategoryId { get; set; }
    //    public string? CategoryName { get; set; }
    //    public double AverageRating { get; set; }
    //    public int ReviewCount { get; set; }
    //    public List<ProductImageDto> Images { get; set; }
    //    public List<ProductOptionDto> Options { get; set; }
    //    public List<ProductVariantDto> Variants { get; set; }
    //    public List<PriceTierDto> PriceTiers { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //    public ProductDto new 



    //}

    public record CreateProductDto(string Name, string Description, Guid CategoryId,
    int MinimumOrderQuantity, decimal BasePrice, string currency);

    //public record ProductSummaryDto(Guid Id, string Name ,string? MainImageUrl, decimal BasePrice, string Currency, 
    //    int MinimumOrderQuantity, string Status, Guid CompanyId, string? CompanyName, Guid CategoryId, string? CategoryName,
    //    double AverageRating, int ReviewCount, DateTime CreatedAt);

    public class ProductSummaryDto
    {
        public ProductSummaryDto()
        {
            
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? MainImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public int Status { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

        public record UpdateProductDto(string Name, string Description, Guid CategoryId, int MinimumOrderQuantity, 
        decimal BasePrice);

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
            p.Id, p.Name, p.Description ,p.MainImageUrl,
            p.BasePrice, p.Currency, p.MinimumOrderQuantity, (int)p.Status,
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
                v.OptionValues.Select(ov => new VariantOptionValueDto(
                    ov.ProductOptionValue.Id,
                    ov.ProductOptionValue.Value,
                    ov.ProductOptionValue.ProductOption.Name)).ToList()))
                .ToList(),
            p.PriceTiers.OrderBy(t => t.MinQuantity)
                .Select(t => new PriceTierDto(t.Id, t.MinQuantity, t.MaxQuantity, t.UnitPrice))
                .ToList(),
            p.CreatedAt, p.UpdatedAt);
    }


}

