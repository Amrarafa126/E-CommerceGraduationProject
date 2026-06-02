using E_Commerce.Core.Features.Orders.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Orders.Commands.Vaildations
{
    public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("معرف الطلب مطلوب.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("سبب الإلغاء مطلوب.")
                .MaximumLength(500).WithMessage("سبب الإلغاء لا يجب أن يتجاوز 500 حرف.");
        }
    }
}
