using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public record UpdateProductCommand(
        Guid ProductId, string Name, string Description, Guid CategoryId,
        int MinimumOrderQuantity, decimal BasePrice, int StockQuantity,
        string? Brand = null, string? ModelNumber = null, string? OriginCountry = null,
        int? UnitOfMeasure = null, int? LeadTimeDays = null, int? LeadTimeDaysSample = null,
        string? SupplyAbility = null, string? TradeTerms = null,
        string? PortOfLoading = null, string? PaymentTerms = null, string? PackagingDetails = null,
        bool? SampleAvailable = null, decimal? SamplePrice = null, int? SampleMoq = null,
        string? MetaTitle = null, string? MetaDescription = null, string? Slug = null)
        : IRequest<ApiResponse<ProductDto>>;
}
