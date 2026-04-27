using E_Commerce.Api.Base;
using E_Commerce.Core.Features.Authentication.Commands.Models;
using E_Commerce.Core.Features.Companies;
using E_Commerce.Core.Features.Companies.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController(ISender mediator) : ControllerBase
    {
        /// <summary>Register a new Supplier account</summary>
        [HttpPost("register/Seller")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RegisterSeller(
            [FromBody] RegisterSellerDto dto,
            CancellationToken ct)
        {
            var result = await mediator.Send(
                new RegisterSellerCommand(dto.Email, dto.Password, dto.FirstName, dto.LastName, dto.PhoneNumber
                , dto.CompanyName, dto.CompanyDescription, dto.Street,
                dto.City, dto.State, dto.Country, dto.PostalCode, dto.ContactEmail,
                dto.ContactPhone, dto.YearEstablished, dto.EmployeesCount),
                ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
