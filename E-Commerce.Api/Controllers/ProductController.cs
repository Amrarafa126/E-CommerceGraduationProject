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

        [HttpPost]
        [Authorize(Roles = "Supplier")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(
        [FromBody] CreateProductDto dto,
        CancellationToken ct)
        {
            var result = await mediator.Send(new AddProductModelComands(
                dto.Name, dto.Description, dto.CategoryId,
                dto.MinimumOrderQuantity, dto.BasePrice, dto.currency), ct);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("{id:guid}/options")]
        [Authorize(Roles = "Supplier")]
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
    [Authorize(Roles = "Supplier")]
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
        [Authorize(Roles = "Supplier")]
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
            //public ProductController(IMediator mediator)
            //{
            //    _mediator = mediator;
            //}
            //[HttpPost("ADD product")]
            //public async Task<IActionResult> AddProduct([FromForm] AddProductModelComands command)
            //{
            //    var Response = await _mediator.Send(command);
            //    return Ok(Response);
            //}
            //[HttpGet("Get-List-HomePage")]
            //public async Task<IActionResult> GetProductList()
            //{
            //    var Response = await _mediator.Send(new GetListProductQueries());
            //    return Ok(Response);
            //}
            //[HttpPost("products/{id}/options")]
            //public async Task<IActionResult> AddOption(int id, AddProductOptionCommand command)
            //{
            //    command.ProductId = id;
            //    return Ok(await _mediator.Send(command));
            //}
            //[HttpPost("options/{id}/values")]
            //public async Task<IActionResult> AddValue(int id, AddOptionValueCommand command)
            //{
            //    if (command.ProductOptionId != id)
            //        return BadRequest("Not Found ProductOptionValue");

            //    command.ProductOptionId = id;
            //    return Ok(await _mediator.Send(command));
            //}

            //[HttpPost("products/{id}/price-tiers")]
            //public async Task<IActionResult> AddPriceTier(int id, AddPriceTierCommand command)
            //{
            //    command.ProductId = id;
            //    return Ok(await _mediator.Send(command));
            //}
            //[HttpPost("generate-variants/{productId}")]
            //public async Task<IActionResult> GenerateVariants(int productId)
            //{
            //    var result = await _mediator.Send(new GenerateVariantsCommand
            //    {
            //        ProductId = productId
            //    });

            //    return Ok(result);
            //}
       

