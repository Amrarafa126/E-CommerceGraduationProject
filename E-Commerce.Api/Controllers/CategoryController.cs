using E_Commerce.Core.Features.Categorys;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.Categorys.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController(ISender mediator) : ControllerBase
    {
        

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), 200)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var r = await mediator.Send(new GetCategoriesQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new GetCategoryByIdQuery(id), ct);
            return StatusCode(r.StatusCode, r);
        }

     
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new CreateCategoryCommand(
                dto.Name, dto.Description, dto.ParentCategoryId), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new UpdateCategoryCommand(
                id, dto.Name, dto.Description), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new DeleteCategoryCommand(id), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
}