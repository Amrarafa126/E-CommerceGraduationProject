using E_Commerce.Core.Features.Payment.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Payment.Commands.Validation
{
    public class InitiatePaymentValidator : AbstractValidator<InitiatePaymentCommand>
    {
        public InitiatePaymentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("معرف الطلب مطلوب.");
            RuleFor(x => x.PaymentMethod)
                .InclusiveBetween(0, 3)
                .WithMessage("طريقة الدفع يجب أن تكون 0 (بطاقة)، 1 (محفظة)، 2 (نقداً عند الاستلام)، أو 3 (تحويل بنكي).");
            RuleFor(x => x.CardToken)
                .NotEmpty().WithMessage("رمز البطاقة مطلوب عند الدفع بالبطاقة.")
                .When(x => x.PaymentMethod == 0);
        }
    }
}
