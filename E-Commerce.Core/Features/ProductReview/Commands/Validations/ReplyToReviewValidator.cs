using E_Commerce.Core.Features.ProductReview.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.ProductReview.Commands.Validations
{
    public class ReplyToReviewValidator : AbstractValidator<ReplyToReviewCommand>
    {
        public ReplyToReviewValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty().WithMessage("معرف التقييم مطلوب.");
            RuleFor(x => x.Reply)
                .NotEmpty().WithMessage("الرد مطلوب.")
                .MaximumLength(1000).WithMessage("الرد لا يجب أن يتجاوز 1000 حرف.");
        }
    }
}
