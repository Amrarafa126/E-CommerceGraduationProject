using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Companies
{
    public class CompanyDto {
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? LogoUrl { get; set; }
    public int YearEstablished { get; set; }
    public int EmployeesCount { get; set; }
    public int Status { get; set; }
    public Guid OwnerUserId { get; set; }
    public string OwnerName { get; set; } = "";
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Country { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string ContactEmail { get; set; } = "";
    public string ContactPhone { get; set; } = "";
    public decimal WalletPendingBalance { get; set; }
    public decimal WalletAvailableBalance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
    public class CompanySummaryDto {
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? LogoUrl { get; set; }
    public int Status { get; set; }
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public int YearEstablished { get; set; }
    public int EmployeesCount { get; set; }
}
    public record CreateCompanyDto(string Name, string Description, string Street, string City, string State, string Country,
        string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);
    public record UpdateCompanyDto(string Name, string Description, string Street, string City, string State, string Country,
        string PostalCode, string ContactEmail, string ContactPhone, int YearEstablished, int EmployeesCount);

}
