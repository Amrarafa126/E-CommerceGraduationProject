using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.Categorys.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetCategoryList()
        {
            var Response = await _mediator.Send(new GetListCategoryQueries());
            return Ok(Response);
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCategory(AddCategoryCommand command)
        {
            var Response = await _mediator.Send(command);
            return Ok(Response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> EditCategory(EditCategoryCommand command)
        {
            var Response = await _mediator.Send(command);
            return Ok(Response);
        }
        [HttpDelete("delete{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var Response = await _mediator.Send(new DeleteCategoryCommand(id));
            return Ok(Response);
        }

    }
}