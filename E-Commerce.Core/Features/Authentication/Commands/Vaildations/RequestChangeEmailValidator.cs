using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class RequestChangeEmailValidator : AbstractValidator<RequestChangeEmailCommand>
    {
        public RequestChangeEmailValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("البريد الإلكتروني الجديد مطلوب.")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح.");
        }
    }
}
