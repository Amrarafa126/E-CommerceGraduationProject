using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class RegisterSellerValidator : AbstractValidator<RegisterSellerCommand>
    {
        public RegisterSellerValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح.")
                .MaximumLength(256).WithMessage("البريد الإلكتروني لا يجب أن يتجاوز 256 حرف.");
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
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("اسم الشركة مطلوب.")
                .MaximumLength(200).WithMessage("اسم الشركة لا يجب أن يتجاوز 200 حرف.");
            RuleFor(x => x.CompanyDescription)
                .NotEmpty().WithMessage("وصف الشركة مطلوب.")
                .MaximumLength(2000).WithMessage("وصف الشركة لا يجب أن يتجاوز 2000 حرف.");
            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("بريد التواصل مطلوب.")
                .EmailAddress().WithMessage("بريد التواصل غير صحيح.");
            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("هاتف التواصل مطلوب.")
                .MaximumLength(20).WithMessage("هاتف التواصل لا يجب أن يتجاوز 20 رقم.");
            RuleFor(x => x.YearEstablished)
                .InclusiveBetween(1800, DateTime.UtcNow.Year).WithMessage($"سنة التأسيس يجب أن تكون بين 1800 و{DateTime.UtcNow.Year}.");
            RuleFor(x => x.EmployeesCount)
                .GreaterThan(0).WithMessage("عدد الموظفين يجب أن يكون أكبر من صفر.");
        }
    }
}
