using E_Commerce.Core.Features.Shippings.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Shippings.Commands.Vaildations
{
    public class CreateShipmentValidator : AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("معرف الطلب مطلوب.");
            RuleFor(x => x.RecipientName)
                .NotEmpty().WithMessage("اسم المستلم مطلوب.")
                .MaximumLength(200).WithMessage("اسم المستلم لا يجب أن يتجاوز 200 حرف.");
            RuleFor(x => x.AddressLine1)
                .NotEmpty().WithMessage("عنوان الشحن مطلوب.")
                .MaximumLength(300).WithMessage("عنوان الشحن لا يجب أن يتجاوز 300 حرف.");
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("المدينة مطلوبة.")
                .MaximumLength(100).WithMessage("المدينة لا يجب أن تتجاوز 100 حرف.");
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("المحافظة مطلوبة.")
                .MaximumLength(100).WithMessage("المحافظة لا يجب أن تتجاوز 100 حرف.");
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("الدولة مطلوبة.")
                .MaximumLength(100).WithMessage("الدولة لا يجب أن تتجاوز 100 حرف.");
            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("الرمز البريدي مطلوب.")
                .MaximumLength(20).WithMessage("الرمز البريدي لا يجب أن يتجاوز 20 حرف.");
            RuleFor(x => x.Method)
                .InclusiveBetween(0, 3)
                .WithMessage("طريقة الشحن يجب أن تكون 0 (عادي)، 1 (سريع)، 2 (فوري)، أو 3 (استلام).");
            RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0).WithMessage("تكلفة الشحن يجب أن تكون صفر أو أكبر.");
        }
    }
}
