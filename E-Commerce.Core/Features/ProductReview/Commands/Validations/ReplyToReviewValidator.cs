using E_Commerce.Core.Features.ProductReview.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Commands.Validations
{
    public class ReplyToReviewValidator : AbstractValidator<ReplyToReviewCommand>
    {
        public ReplyToReviewValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.Reply).NotEmpty().MaximumLength(1000);
        }
    }
}
