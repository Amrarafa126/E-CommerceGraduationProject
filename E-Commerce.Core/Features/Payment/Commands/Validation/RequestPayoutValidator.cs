using E_Commerce.Core.Features.Payment.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Commands.Validation
{
    public class RequestPayoutValidator : AbstractValidator<RequestPayoutCommand>
    {
        public RequestPayoutValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
