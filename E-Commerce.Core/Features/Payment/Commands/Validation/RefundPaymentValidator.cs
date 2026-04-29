using E_Commerce.Core.Features.Payment.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Commands.Validation
{
    public class RefundPaymentValidator : AbstractValidator<RefundPaymentCommand>
    {
        public RefundPaymentValidator()
        {
            RuleFor(x => x.PaymentId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }
}
