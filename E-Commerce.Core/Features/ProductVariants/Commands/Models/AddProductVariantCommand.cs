using MediatR;


namespace E_Commerce.Core.Features.ProductVariants.Commands.Models
{
    public record AddProductVariantCommand(
     Guid ProductId,string SKU, decimal Price, int StockQuantity, List<Guid> OptionValueIds)
     : IRequest<ApiResponse<ProductVariantDto>>;
}
