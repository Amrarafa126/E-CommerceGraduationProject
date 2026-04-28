
namespace E_Commerce.Core.Features.ProductVariants
{
    public record ProductVariantDto(Guid Id, string SKU, decimal Price, int StockQuantity, bool IsActive, 
        List<VariantOptionValueDto> OptionValues);

    public record AddProductVariantDto(string SKU, decimal Price, int StockQuantity,
    List<Guid> OptionValueIds);

    public record VariantOptionValueDto(Guid Id, string Value, string OptionName);



}
