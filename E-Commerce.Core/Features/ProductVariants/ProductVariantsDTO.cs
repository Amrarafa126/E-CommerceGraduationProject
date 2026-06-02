
namespace E_Commerce.Core.Features.ProductVariants
{
    public record ProductVariantDto(Guid Id, string SKU, decimal Price, int StockQuantity, bool IsActive,
        string? Barcode, string? ImageUrl,
        List<VariantOptionValueDto> OptionValues);

    public record AddProductVariantDto(string SKU, decimal Price, int StockQuantity,
        List<Guid> OptionValueIds, string? Barcode = null, string? ImageUrl = null);

    public record VariantOptionValueDto(Guid Id, string Value, string OptionName);

    public record UpdateProductVariantDto(string SKU, decimal Price, int StockQuantity,
        bool IsActive, List<Guid>? OptionValueIds, string? Barcode = null, string? ImageUrl = null);
}
