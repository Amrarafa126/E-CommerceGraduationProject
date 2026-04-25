using E_Commerce.Core.Features.ProductVariantsValue;

namespace E_Commerce.Core.Features.ProductVariants
{
    public record ProductVariantDto(Guid Id, string SKU, decimal Price, int StockQuantity,
        List<VariantOptionValueDto> OptionValues);

    public record AddProductVariantDto(string SKU, decimal Price, int StockQuantity,
    List<Guid> OptionValueIds);

}
