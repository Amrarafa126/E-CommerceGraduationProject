using E_Commerce.Core.Features.Authentication;
using E_Commerce.Core.Features.Authentication.Commands.Models;
using E_Commerce.Core.Features.Authentication.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthenticationController(ISender mediator) : ControllerBase
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

        [HttpPost("magic-link")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> RequestMagicLink(
            [FromBody] RequestMagicLinkDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new RequestMagicLinkCommand(dto.Email), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("magic-link/verify")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> VerifyMagicLink(
            [FromQuery] string email, [FromQuery] string token, CancellationToken ct)
        {
            var r = await mediator.Send(new VerifyMagicLinkCommand(email, token), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("send-verification-email")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> SendVerificationEmail(CancellationToken ct)
        {
            var r = await mediator.Send(new SendEmailVerificationCommand(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("verify-email")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> VerifyEmail(
            [FromQuery] Guid userId, [FromQuery] string token, CancellationToken ct)
        {
            var r = await mediator.Send(new VerifyEmailCommand(userId, token), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var r = await mediator.Send(new GetCurrentUserQuery(), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new UpdateProfileCommand(
                dto.FirstName, dto.LastName, dto.PhoneNumber, dto.AvatarUrl), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto, CancellationToken ct)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest(ApiResponse<object>.Fail("كلمة المرور الجديدة وتأكيدها غير متطابقين."));

            var r = await mediator.Send(new ChangePasswordCommand(dto.CurrentPassword, dto.NewPassword), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new ForgotPasswordCommand(dto.Email), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto dto, CancellationToken ct)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest(ApiResponse<object>.Fail("كلمة المرور الجديدة وتأكيدها غير متطابقين."));

            var r = await mediator.Send(new ResetPasswordCommand(dto.Email, dto.Token, dto.NewPassword), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("change-email-request")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        public async Task<IActionResult> RequestChangeEmail(
            [FromBody] RequestChangeEmailDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new RequestChangeEmailCommand(dto.NewEmail), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("confirm-change-email")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ConfirmChangeEmail(
            [FromQuery] Guid userId, [FromQuery] string newEmail, [FromQuery] string token, CancellationToken ct)
        {
            var r = await mediator.Send(new ConfirmChangeEmailCommand(userId, newEmail, token), ct);
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

        [HttpPost("google")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(new GoogleLoginCommand(dto.IdToken), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
}
