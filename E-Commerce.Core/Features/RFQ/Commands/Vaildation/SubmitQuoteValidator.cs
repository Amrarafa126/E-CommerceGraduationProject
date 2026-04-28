using E_Commerce.Core.Features.RFQ.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ.Commands.Vaildation
{
    public class SubmitQuoteValidator : AbstractValidator<SubmitQuoteCommand>
    {
        public SubmitQuoteValidator()
        {
            RuleFor(x => x.RfqRequestId).NotEmpty();
            RuleFor(x => x.UnitPrice).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.ValidityDays).InclusiveBetween(1, 90);
        }
    }
}
