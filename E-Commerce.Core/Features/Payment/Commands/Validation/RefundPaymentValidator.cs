using E_Commerce.Core.Features.Payment.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Payment.Commands.Validation
{
    public class RefundPaymentValidator : AbstractValidator<RefundPaymentCommand>
    {
        public RefundPaymentValidator()
        {
            RuleFor(x => x.PaymentId).NotEmpty().WithMessage("معرف الدفع مطلوب.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("المبلغ يجب أن يكون أكبر من صفر.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("سبب الاسترجاع مطلوب.")
                .MaximumLength(500).WithMessage("سبب الاسترجاع لا يجب أن يتجاوز 500 حرف.");
        }
    }
}
