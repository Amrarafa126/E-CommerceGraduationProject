using E_Commerce.Core.Features.Category.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IMediator _mediator;

        public ValuesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetStudentList()
        {
            var Response = await _mediator.Send(new GetListCategoryQueries());
            return Ok(Response);
        }
    }
}
