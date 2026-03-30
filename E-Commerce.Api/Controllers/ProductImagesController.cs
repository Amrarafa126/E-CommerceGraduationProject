using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.ProductImages.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {

        IMediator _mediator;
        public ProductImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Add-Product-Images")]
        public async Task<IActionResult> AddProductImages([FromForm] AddProductImagesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpDelete("Delete-Product-Image{id}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteProductImageCommand(id));
            return Ok(result);
        }
        [HttpPut("Edit-Product-Image")]
        public async Task<IActionResult> EditProductImage(UpdateProductImageCommand command)
        {
            var Response = await _mediator.Send(command);
            return Ok(Response);
        }
    }
}
