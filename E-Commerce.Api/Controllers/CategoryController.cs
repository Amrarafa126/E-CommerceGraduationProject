using E_Commerce.Core.Features.Categorys;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.Categorys.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ISender mediator) : ControllerBase
    {
        

        [HttpGet("Get-All-Categories")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), 200)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var r = await mediator.Send(new GetCategoriesQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("Get-By-Id/Category/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new GetCategoryByIdQuery(id), ct);
            return StatusCode(r.StatusCode, r);
        }

     
        [HttpPost("Create-Category")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new CreateCategoryCommand(
                dto.Name, dto.Description, dto.ParentCategoryId), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
}