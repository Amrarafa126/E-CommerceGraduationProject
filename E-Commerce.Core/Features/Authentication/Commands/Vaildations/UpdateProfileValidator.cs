using E_Commerce.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Authentication.Commands.Vaildations
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("الاسم الأول مطلوب.")
                .MaximumLength(100).WithMessage("الاسم الأول لا يجب أن يتجاوز 100 حرف.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("اسم العائلة مطلوب.")
                .MaximumLength(100).WithMessage("اسم العائلة لا يجب أن يتجاوز 100 حرف.");
            RuleFor(x => x.AvatarUrl)
                .MaximumLength(1000).WithMessage("رابط الصورة غير صالح.")
                .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
        }
    }
}
