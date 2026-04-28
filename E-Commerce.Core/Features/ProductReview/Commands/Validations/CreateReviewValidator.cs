using E_Commerce.Core.Features.ProductReview.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductReview.Commands.Validations
{

    public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Comment).NotEmpty().MaximumLength(2000);
        }
    }
}
