using E_Commerce.Core.Features.ProductImages;
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductPriceTiers;
using E_Commerce.Core.Features.ProductVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products
{

    public record ProductDto(Guid Id, string Name, string Description,
        string? MainImageUrl, decimal BasePrice, string Currency, int MinimumOrderQuantity, string Status,
        Guid CompanyId, string? CompanyName, Guid CategoryId, string? CategoryName, double AverageRating,
        int ReviewCount, List<ProductImageDto> Images, List<ProductOptionDto> Options, List<ProductVariantDto> Variants,
        List<PriceTierDto> PriceTiers, DateTime CreatedAt, DateTime? UpdatedAt);

    public record CreateProductDto(string Name, string Description, Guid CategoryId,
    int MinimumOrderQuantity, decimal BasePrice, string currency);

    public record ProductSummaryDto(Guid Id, string Name, string? Description,
    string? MainImageUrl, decimal BasePrice, string Currency, int MinimumOrderQuantity,
    string Status, Guid CompanyId, string? CompanyName, Guid CategoryId, string? CategoryName,
    double AverageRating, int ReviewCount, DateTime CreatedAt);
    public record UpdateProductDto(string Name, string Description, Guid CategoryId, int MinimumOrderQuantity, 
        decimal BasePrice);

}

