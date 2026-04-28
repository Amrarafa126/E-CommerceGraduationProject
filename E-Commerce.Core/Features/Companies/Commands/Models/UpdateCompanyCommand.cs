using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Companies.Commands.Models
{
    public record UpdateCompanyCommand(
     Guid CompanyId,
     string CompanyName, string CompanyDescription,
     string Street, string City, string State, string Country, string PostalCode,
     string ContactEmail, string ContactPhone,
     int YearEstablished, int EmployeesCount)
     : IRequest<ApiResponse<CompanyDto>>;
}
