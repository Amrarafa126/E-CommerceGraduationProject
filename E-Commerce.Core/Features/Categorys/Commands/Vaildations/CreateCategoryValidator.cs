using E_Commerce.Core.Features.Categorys.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Categorys.Commands.Vaildations
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم الفئة مطلوب.")
                .MaximumLength(200).WithMessage("اسم الفئة لا يجب أن يتجاوز 200 حرف.");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("الوصف لا يجب أن يتجاوز 1000 حرف.")
                .When(x => x.Description != null);
        }
    }
}
