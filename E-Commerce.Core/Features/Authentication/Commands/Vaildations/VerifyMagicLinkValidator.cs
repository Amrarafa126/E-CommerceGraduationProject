using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class VerifyMagicLinkValidator : AbstractValidator<VerifyMagicLinkCommand>
    {
        public VerifyMagicLinkValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح.");
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("الرمز مطلوب.");
        }
    }
}
