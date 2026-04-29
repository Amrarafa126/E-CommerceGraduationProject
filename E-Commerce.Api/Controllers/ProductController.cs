using E_Commerce.Core.Features.Categorys.Queries.Models;
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Core.Features.ProductOptionsValue.Commands.Models;
using E_Commerce.Core.Features.ProductPriceTiers;
using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Core.Features.Products.Queries.Models;
using E_Commerce.Core.Features.ProductVariants;
using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ISender Mediator) : ControllerBase
    {

        /// <summary>Get all published products (paginated + filtered)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? categoryId,
            [FromQuery] Guid? companyId,
            [FromQuery] string? search,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(
                new GetProductsQuery(categoryId, companyId, search, minPrice, maxPrice, page, pageSize), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Get my products (seller only, paginated)</summary>
        [HttpGet("my")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(new GetMyProductsQuery(page, pageSize), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Get full product details by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(new GetProductByIdQuery(id), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Create a new product (seller only)</summary>
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new CreateProductCommand(dto.Name, dto.Description, dto.CategoryId,
                    dto.MinimumOrderQuantity, dto.BasePrice, dto.currency), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Update an existing product (seller who owns it, or Admin)</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new UpdateProductCommand(id, dto.Name, dto.Description,
                    dto.CategoryId, dto.MinimumOrderQuantity, dto.BasePrice), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Soft-delete a product (seller who owns it, or Admin)</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(new DeleteProductCommand(id), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Publish a product so it becomes visible to buyers</summary>
        [HttpPatch("{id:guid}/publish")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(new PublishProductCommand(id), ct);
            return StatusCode(result.StatusCode, result);
        }

        // ══════════════════════════════════════════════════════════════════
        //  PRODUCT OPTIONS  — CRUD
        //  Route: /api/product/{id}/options
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Add a new option to a product (e.g. "Color", "Size")</summary>
        [HttpPost("{id:guid}/options")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductOptionDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddOption(
            Guid id,
            [FromBody] AddProductOptionDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new AddProductOptionCommand(id, dto.Name, dto.DisplayOrder, dto.Values), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Update an existing option name / display order</summary>
        [HttpPut("{id:guid}/options/{optionId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductOptionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOption(
            Guid id,
            Guid optionId,
            [FromBody] UpdateProductOptionDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new UpdateProductOptionCommand(id, optionId, dto.Name, dto.DisplayOrder), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Delete an option and all its values (blocked if variants reference them)</summary>
        [HttpDelete("{id:guid}/options/{optionId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteOption(Guid id, Guid optionId, CancellationToken ct)
        {
            var result = await Mediator.Send(new DeleteProductOptionCommand(id, optionId), ct);
            return StatusCode(result.StatusCode, result);
        }

        // ══════════════════════════════════════════════════════════════════
        //  OPTION VALUES  — CRUD
        //  Route: /api/product/{id}/options/{optionId}/values
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Add a new value to an option (e.g. add "XL" to the "Size" option)</summary>
        //[HttpPost("{id:guid}/options/{optionId:guid}/values")]
        //[Authorize(Roles = "Seller,Admin")]
        //[ProducesResponseType(typeof(ApiResponse<ProductOptionValueDto>), StatusCodes.Status201Created)]
        //public async Task<IActionResult> AddOptionValue(
        //    Guid id,
        //    Guid optionId,
        //    [FromBody] AddOptionValueDto dto,
        //    CancellationToken ct)
        //{
        //    var result = await Mediator.Send(
        //        new AddOptionValueCommand(id, optionId, dto.Value, dto.DisplayOrder), ct);
        //    return StatusCode(result.StatusCode, result);
        //}

        /// <summary>Update an option value label / display order</summary>
        [HttpPut("{id:guid}/options/{optionId:guid}/values/{valueId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductOptionValueDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOptionValue(
            Guid id,
            Guid optionId,
            Guid valueId,
            [FromBody] UpdateOptionValueDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new UpdateOptionValueCommand(id, optionId, valueId, dto.Value, dto.DisplayOrder), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Delete a single option value (blocked if any variant uses it)</summary>
        [HttpDelete("{id:guid}/options/{optionId:guid}/values/{valueId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteOptionValue(
            Guid id, Guid optionId, Guid valueId, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new DeleteOptionValueCommand(id, optionId, valueId), ct);
            return StatusCode(result.StatusCode, result);
        }

        // ══════════════════════════════════════════════════════════════════
        //  PRICE TIERS  — CRUD
        //  Route: /api/product/{id}/price-tiers
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Add a new bulk-pricing tier</summary>
        [HttpPost("{id:guid}/price-tiers")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<PriceTierDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddPriceTier(
            Guid id,
            [FromBody] AddPriceTierDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new AddPriceTierCommand(id, dto.MinQuantity, dto.UnitPrice, dto.MaxQuantity), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Update an existing price tier</summary>
        [HttpPut("{id:guid}/price-tiers/{tierId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<PriceTierDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePriceTier(
            Guid id,
            Guid tierId,
            [FromBody] UpdatePriceTierDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new UpdatePriceTierCommand(id, tierId, dto.MinQuantity, dto.UnitPrice, dto.MaxQuantity), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Delete a price tier</summary>
        [HttpDelete("{id:guid}/price-tiers/{tierId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePriceTier(Guid id, Guid tierId, CancellationToken ct)
        {
            var result = await Mediator.Send(new DeletePriceTierCommand(id, tierId), ct);
            return StatusCode(result.StatusCode, result);
        }

        // ══════════════════════════════════════════════════════════════════
        //  VARIANTS  — CRUD
        //  Route: /api/product/{id}/variants
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Add a new variant (SKU + price + stock + option-value combinations)</summary>
        [HttpPost("{id:guid}/variants")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddVariant(
            Guid id,
            [FromBody] AddProductVariantDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new AddProductVariantCommand(id, dto.SKU, dto.Price, dto.StockQuantity,
                    dto.OptionValueIds ?? new()), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Update variant details (price, stock, active state, option-value links)</summary>
        [HttpPut("{id:guid}/variants/{variantId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateVariant(
            Guid id,
            Guid variantId,
            [FromBody] UpdateProductVariantDto dto,
            CancellationToken ct)
        {
            var result = await Mediator.Send(
                new UpdateProductVariantCommand(id, variantId, dto.SKU, dto.Price,
                    dto.StockQuantity, dto.IsActive, dto.OptionValueIds ?? new()), ct);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Delete a variant</summary>
        [HttpDelete("{id:guid}/variants/{variantId:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVariant(Guid id, Guid variantId, CancellationToken ct)
        {
            var result = await Mediator.Send(new DeleteProductVariantCommand(id, variantId), ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
          