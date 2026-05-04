using E_Commerce.Core.Features.Authentication;
using E_Commerce.Core.Features.Authentication.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController(ISender mediator) : ControllerBase
    {
        /// <summary>Register a new Supplier account</summary>
        [HttpPost("register/seller")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]  // validation failed
        public async Task<IActionResult> RegisterSeller(
        [FromBody] RegisterSellerDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new RegisterSellerCommand(
                dto.Email, dto.Password, dto.FirstName, dto.LastName, dto.PhoneNumber,
                dto.CompanyName, dto.CompanyDescription,
                dto.Street, dto.City, dto.State, dto.Country, dto.PostalCode,
                dto.ContactEmail, dto.ContactPhone,
                dto.YearEstablished, dto.EmployeesCount), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("register/Buyer")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RegisterBuyer(
           [FromBody] RegisterBuyerDto dto,
           CancellationToken ct)
        {
            var result = await mediator.Send(
                new RegisterBuyerCommand(dto.Email, dto.Password, dto.FirstName, dto.LastName, dto.PhoneNumber),
                ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Login(
        [FromBody] LoginDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new LoginCommand(dto.Email, dto.Password), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(
                new RefreshTokenCommand(dto.AccessToken, dto.RefreshToken), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var result = await mediator.Send(new LogoutCommand(), ct);
            return StatusCode(result.StatusCode, result);
        }

        //[HttpGet("me")]
        //[Authorize]
        //[ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        //[ProducesResponseType(typeof(ApiResponse<object>), 401)]
        //public async Task<IActionResult> Me(CancellationToken ct)
        //{
        //    var r = await mediator.Send(new GetCurrentUserQuery(), ct);
        //    return StatusCode(r.StatusCode, r);
        //}
    }
}
