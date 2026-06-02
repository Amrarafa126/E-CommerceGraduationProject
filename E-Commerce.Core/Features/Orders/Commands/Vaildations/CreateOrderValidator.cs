using E_Commerce.Core.Features.Orders.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Orders.Commands.Vaildations
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.SellerCompanyId).NotEmpty().WithMessage("معرف شركة البائع مطلوب.");
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("العملة مطلوبة.")
                .Length(3).WithMessage("رمز العملة يجب أن يكون 3 أحرف.");
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("الطلب يجب أن يحتوي على منتج واحد على الأقل.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("معرف المنتج مطلوب.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من صفر.");
            });
        }
    }
}
