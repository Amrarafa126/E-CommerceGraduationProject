using E_Commerce.Core.Features.ProductReview.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.ProductReview.Commands.Validations
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("معرف المنتج مطلوب.");
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("التقييم يجب أن يكون بين 1 و 5.");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان التقييم مطلوب.")
                .MaximumLength(200).WithMessage("العنوان لا يجب أن يتجاوز 200 حرف.");
            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("التعليق مطلوب.")
                .MaximumLength(2000).WithMessage("التعليق لا يجب أن يتجاوز 2000 حرف.");
        }
    }
}
