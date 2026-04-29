using E_Commerce.Core.Features.Companies;
using E_Commerce.Core.Features.Companies.Commands.Models;
using E_Commerce.Core.Features.Companies.Queries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/companies")]
    [Produces("application/json")]
    public class CompanyController(ISender mediator) : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<CompanySummaryDto>>), 200)]
        public async Task<IActionResult> GetAll(
       [FromQuery] string? search,
       [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
       CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetCompaniesQuery(search, page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get company by ID (includes wallet balances for owner).</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new GetCompanyByIdQuery(id), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get the authenticated seller's own company.</summary>
        [HttpGet("my")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            var r = await mediator.Send(new GetMyCompanyQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Update company info (Seller owner or Admin only).</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new UpdateCompanyCommand(
                id, dto.Name, dto.Description,
                dto.Street, dto.City, dto.State, dto.Country, dto.PostalCode,
                dto.ContactEmail, dto.ContactPhone,
                dto.YearEstablished, dto.EmployeesCount), ct);
            return StatusCode(r.StatusCode, r);
        }

    }
}
