using E_Commerce.Core.Features.RFQ.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.RFQ.Commands.Vaildation
{
    public class SubmitQuoteValidator : AbstractValidator<SubmitQuoteCommand>
    {
        public SubmitQuoteValidator()
        {
            RuleFor(x => x.RfqRequestId).NotEmpty().WithMessage("معرف طلب التسعير مطلوب.");
            RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("سعر الوحدة يجب أن يكون أكبر من صفر.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من صفر.");
            RuleFor(x => x.ValidityDays)
                .InclusiveBetween(1, 90).WithMessage("مدة الصلاحية يجب أن تكون بين 1 و 90 يوم.");
        }
    }
}
