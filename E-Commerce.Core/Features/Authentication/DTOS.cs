using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Authentication
{
    //public record UserDto(Guid Id, string Email, string FirstName, string LastName, string FullName, string? PhoneNumber, 
    //    string Role, Guid? OwnedCompanyId, bool IsEmailVerified, bool IsActive, DateTime CreatedAt);

    public class UserDto
    {
        public Guid Id { get; init; }
            public string Email { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string FullName => $"{FirstName} {LastName}".Trim();
            public string? PhoneNumber { get; init; }
            public string Role { get; init; }
            public Guid? OwnedCompanyId { get; init; }
            public bool IsEmailVerified { get; init; }
            public bool IsActive { get; init; }
            public DateTime CreatedAt { get; init; }
    
            //public UserDto(Guid id, string email, string firstName, string lastName, string? phoneNumber, 
            //    string role, Guid? ownedCompanyId, bool isEmailVerified, bool isActive, DateTime createdAt)
            //{
            //    Id = id;
            //    Email = email;
            //    FirstName = firstName;
            //    LastName = lastName;
            //    PhoneNumber = phoneNumber;
            //    Role = role;
            //    OwnedCompanyId = ownedCompanyId;
            //    IsEmailVerified = isEmailVerified;
            //    IsActive = isActive;
            //    CreatedAt = createdAt;
        


    }
    public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserDto User);
    public record RegisterSellerDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber, string CompanyName, string CompanyDescription, string Street, string City, string State, string Country, string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);
    public record RegisterBuyerDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber);
    public record LoginDto(string Email, string Password);
    public record RefreshTokenDto(string AccessToken, string RefreshToken);
    public record ChangePasswordDto(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
}
