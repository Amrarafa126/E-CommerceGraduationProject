using E_Commerce.Core.Features.Payment.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Commands.Validation
{

    public class InitiatePaymentValidator : AbstractValidator<InitiatePaymentCommand>
    {
        public InitiatePaymentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.PaymentMethod).InclusiveBetween(0, 3)
                .WithMessage("PaymentMethod must be 0 (Card), 1 (Wallet), 2 (CashOnDelivery), or 3 (BankTransfer).");
            RuleFor(x => x.CardToken)
                .NotEmpty().When(x => x.PaymentMethod == 0)
                .WithMessage("CardToken is required for card payments.");
        }
    }
}
