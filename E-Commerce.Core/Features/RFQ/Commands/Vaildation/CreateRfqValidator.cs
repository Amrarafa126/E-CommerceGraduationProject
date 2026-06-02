using E_Commerce.Core.Features.RFQ.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.RFQ.Commands.Vaildation
{
    public class CreateRfqValidator : AbstractValidator<CreateRfqCommand>
    {
        public CreateRfqValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("العنوان مطلوب.")
                .MaximumLength(300).WithMessage("العنوان لا يجب أن يتجاوز 300 حرف.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("الوصف مطلوب.")
                .MaximumLength(3000).WithMessage("الوصف لا يجب أن يتجاوز 3000 حرف.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من صفر.");
            RuleFor(x => x.SellerCompanyId).NotEmpty().WithMessage("معرف شركة البائع مطلوب.");
            RuleFor(x => x.DeadlineDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("تاريخ الانتهاء يجب أن يكون في المستقبل.")
                .When(x => x.DeadlineDate.HasValue);
        }
    }
}
