using E_Commerce.Core.Features.Companies.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Companies.Commands.Vaildations
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.CompanyDescription).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.ContactPhone).NotEmpty().MaximumLength(20);
            RuleFor(x => x.YearEstablished).InclusiveBetween(1800, DateTime.UtcNow.Year);
            RuleFor(x => x.EmployeesCount).GreaterThan(0);
        }
    }
}
