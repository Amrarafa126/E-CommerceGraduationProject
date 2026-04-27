using MediatR;
namespace E_Commerce.Core.Features.Companies.Commands.Models
{
    public record RegisterSellerCommand(
    string Email, string Password, string FirstName, string LastName, string? PhoneNumber,
    string CompanyName, string CompanyDescription,
    string Street, string City, string State, string Country, string PostalCode,
    string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount) : IRequest<ApiResponse<string>>;
}
