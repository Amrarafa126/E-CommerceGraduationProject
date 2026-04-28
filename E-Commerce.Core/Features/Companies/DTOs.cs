using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Companies
{
    public record CompanyDto(Guid Id, string Name, string Description, string? LogoUrl, int YearEstablished,
        int EmployeesCount, string Status, Guid OwnerUserId, string OwnerName, string Street, string City, string State, 
        string Country, string PostalCode, string ContactEmail, string ContactPhone, decimal WalletPendingBalance,
        decimal WalletAvailableBalance, DateTime CreatedAt, DateTime? UpdatedAt);
    public record CompanySummaryDto(Guid Id, string Name, string? LogoUrl, string Status, string City, string Country, int YearEstablished, int EmployeesCount);
    public record CreateCompanyDto(string Name, string Description, string Street, string City, string State, string Country,
        string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);
    public record UpdateCompanyDto(string Name, string Description, string Street, string City, string State, string Country,
        string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);

}
