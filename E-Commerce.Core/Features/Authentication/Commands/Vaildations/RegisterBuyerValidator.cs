using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class RegisterBuyerValidator : AbstractValidator<RegisterBuyerCommand>
    {
        public RegisterBuyerValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
                .MinimumLength(8).WithMessage("يجب أن تكون كلمة المرور 8 أحرف على الأقل.")
                .Matches("[A-Z]").WithMessage("يجب أن تحتوي كلمة المرور على حرف كبير.")
                .Matches("[0-9]").WithMessage("يجب أن تحتوي كلمة المرور على رقم.")
                .Matches("[^a-zA-Z0-9]").WithMessage("يجب أن تحتوي كلمة المرور على رمز خاص.");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("الاسم الأول مطلوب.")
                .MaximumLength(100).WithMessage("الاسم الأول لا يجب أن يتجاوز 100 حرف.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("اسم العائلة مطلوب.")
                .MaximumLength(100).WithMessage("اسم العائلة لا يجب أن يتجاوز 100 حرف.");
        }
    }
}
