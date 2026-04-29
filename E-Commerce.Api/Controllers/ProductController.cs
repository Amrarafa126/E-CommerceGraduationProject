using E_Commerce.Core.Features.Categorys.Queries.Models;
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Core.Features.ProductPriceTiers;
using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Core.Features.ProductVariants;
using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IMediator mediator;

        //[HttpPost]
        //[Authorize(Roles = "Supplier")]
        //[ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        //public async Task<IActionResult> Create(
        //[FromBody] CreateProductDto dto,
        //CancellationToken ct)
        //{
        // //   var result = await mediator.Send(new AddProductModelComands(
        //        dto.Name, dto.Description, dto.CategoryId,
        //        dto.MinimumOrderQuantity, dto.BasePrice, dto.currency), ct);
        //    return StatusCode(result.StatusCode, result);
        //}
        [HttpPost("{id:guid}/options")]
        [Authorize(Roles = "seller")]
        [ProducesResponseType(typeof(ApiResponse<ProductOptionDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddOption(
             Guid id,
         [FromBody] AddProductOptionDto dto,
         CancellationToken ct)
        {
            var result = await mediator.Send(
                new AddProductOptionCommand(id, dto.Name, dto.DisplayOrder, dto.Values), ct);
            return StatusCode(result.StatusCode, result);
        }

            [HttpPost("{id:guid}/variants")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddVariant(
        Guid id,
        [FromBody] AddProductVariantDto dto,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new AddProductVariantCommand(id, dto.SKU, dto.Price, dto.StockQuantity,dto.OptionValueIds?? new() ), ct);
        return StatusCode(result.StatusCode, result);
    }
        [HttpPost("{id:guid}/price-tiers")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PriceTierDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddPriceTier(
    Guid id,
    [FromBody] AddPriceTierDto dto,
    CancellationToken ct)
        {
            var result = await mediator.Send(
                new AddPriceTierCommand(id, dto.MinQuantity, dto.UnitPrice, dto.MaxQuantity), ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
          