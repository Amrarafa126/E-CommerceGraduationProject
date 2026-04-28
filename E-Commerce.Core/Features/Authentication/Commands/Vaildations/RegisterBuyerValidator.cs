using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class RegisterBuyerValidator : AbstractValidator<RegisterBuyerCommand>
    {
        public RegisterBuyerValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Must contain uppercase.")
                .Matches("[0-9]").WithMessage("Must contain a digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain a special character.");
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        }
    }
}
