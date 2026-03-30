using E_Commerce.Core.Features.Companies.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        IMediator _mediator;
        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Add Company")]
        public async Task<IActionResult> AddCompany(AddCompanyCommands addCompanyCommands)
        {
            var result = await _mediator.Send(addCompanyCommands);
            return Ok("Success Add Company");
        }

    }
}
