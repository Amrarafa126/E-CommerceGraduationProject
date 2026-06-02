using E_Commerce.Core.Features.Companies.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Companies.Commands.Vaildations
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty().WithMessage("معرف الشركة مطلوب.");
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
