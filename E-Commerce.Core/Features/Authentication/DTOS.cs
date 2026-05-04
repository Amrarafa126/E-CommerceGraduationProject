using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Authentication
{
    public record UserDto(Guid Id, string Email, string FirstName, string LastName, string FullName, string? PhoneNumber, 
        string Role, Guid? OwnedCompanyId, bool IsEmailVerified, bool IsActive, DateTime CreatedAt);
    public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserDto User);
    public record RegisterSellerDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber, string CompanyName, string CompanyDescription, string Street, string City, string State, string Country, string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);
    public record RegisterBuyerDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber);
    public record LoginDto(string Email, string Password);
    public record RefreshTokenDto(string AccessToken, string RefreshToken);
    public record ChangePasswordDto(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
}
