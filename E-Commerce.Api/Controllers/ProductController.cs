using E_Commerce.Core.Features.Categorys.Queries.Models;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Core.Features.ProductOptionValues.Commands.Models;
using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Core.Features.Products.Queries.Models;
using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("ADD product")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductModelComands command)
        {
            var Response = await _mediator.Send(command);
            return Ok(Response);
        }
        [HttpGet("Get-List-HomePage")]
        public async Task<IActionResult> GetProductList()
        {
            var Response = await _mediator.Send(new GetListProductQueries());
            return Ok(Response);
        }
        [HttpPost("products/{id}/options")]
        public async Task<IActionResult> AddOption(int id, AddProductOptionCommand command)
        {
            command.ProductId = id;
            return Ok(await _mediator.Send(command));
        }
        [HttpPost("options/{id}/values")]
        public async Task<IActionResult> AddValue(int id, AddOptionValueCommand command)
        {
            if (command.ProductOptionId != id)
                return BadRequest("Not Found ProductOptionValue");

            command.ProductOptionId = id;
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("products/{id}/price-tiers")]
        public async Task<IActionResult> AddPriceTier(int id, AddPriceTierCommand command)
        {
            command.ProductId = id;
            return Ok(await _mediator.Send(command));
        }
        [HttpPost("generate-variants/{productId}")]
        public async Task<IActionResult> GenerateVariants(int productId)
        {
            var result = await _mediator.Send(new GenerateVariantsCommand
            {
                ProductId = productId
            });

            return Ok(result);
        }
    }
}
