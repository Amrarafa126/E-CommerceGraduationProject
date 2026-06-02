using MediatR;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public record CreateProductCommand(
        string Name, string Description, Guid CategoryId,
        int MinimumOrderQuantity, decimal BasePrice,
        string Currency = "EGP",
        int StockQuantity = 0,
        string? Brand = null, string? ModelNumber = null, string? OriginCountry = null,
        int? UnitOfMeasure = null, int? LeadTimeDays = null, int? LeadTimeDaysSample = null,
        string? SupplyAbility = null, string? TradeTerms = null,
        string? PortOfLoading = null, string? PaymentTerms = null, string? PackagingDetails = null,
        bool? SampleAvailable = null, decimal? SamplePrice = null, int? SampleMoq = null,
        string? MetaTitle = null, string? MetaDescription = null, string? Slug = null)
        : IRequest<ApiResponse<ProductDto>>;
}
