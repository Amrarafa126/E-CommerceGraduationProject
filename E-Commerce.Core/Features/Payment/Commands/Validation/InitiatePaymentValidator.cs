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
        private static readonly string[] AllowedMethods = ["Card", "Wallet", "CashOnDelivery", "BankTransfer"];

        public InitiatePaymentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.PaymentMethod).NotEmpty()
                .Must(m => AllowedMethods.Contains(m))
                .WithMessage($"PaymentMethod must be one of: {string.Join(", ", AllowedMethods)}");
            RuleFor(x => x.CardToken)
                .NotEmpty().When(x => x.PaymentMethod == "Card")
                .WithMessage("CardToken is required for card payments.");
        }
    }
}
