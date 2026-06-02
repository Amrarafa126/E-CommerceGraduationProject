namespace E_Commerce.Core.Features.Authentication
{
    public class UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string? PhoneNumber { get; init; }
        public string? AvatarUrl { get; init; }
        public string Role { get; init; } = string.Empty;
        public Guid? OwnedCompanyId { get; init; }
        public bool IsEmailVerified { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserDto User);
    public record RegisterSellerDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber, string CompanyName, string CompanyDescription, string Street, string City, string State, string Country, string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);
    public record RegisterBuyerDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber);
    public record LoginDto(string Email, string Password);
    public record RefreshTokenDto(string AccessToken, string RefreshToken);
    public record ChangePasswordDto(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
    public record RequestMagicLinkDto(string Email);
    public record VerifyMagicLinkDto(string Email, string Token);
    public record SendEmailVerificationDto;
    public record VerifyEmailDto(Guid UserId, string Token);
    public record UpdateProfileDto(string FirstName, string LastName, string? PhoneNumber, string? AvatarUrl);
    public record ForgotPasswordDto(string Email);
    public record ResetPasswordDto(string Email, string Token, string NewPassword, string ConfirmNewPassword);
    public record RequestChangeEmailDto(string NewEmail);
    public record ConfirmChangeEmailDto(Guid UserId, string NewEmail, string Token);
    public record GoogleLoginDto(string IdToken);
}
