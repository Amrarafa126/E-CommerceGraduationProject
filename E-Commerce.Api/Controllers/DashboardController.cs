using E_Commerce.Core.Features.DashboardComapny;
using E_Commerce.Core.Features.DashboardComapny.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    [Authorize(Roles = "Seller,Admin")]
    [Produces("application/json")]
    public class DashboardController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<CompanyDashboardDto>), 200)]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var r = await mediator.Send(new GetDashboardQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
}
