using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح.");
        }
    }
}
