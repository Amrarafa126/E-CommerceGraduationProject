using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductVariants.Commands.Vaildations
{
    public class AddProductVariantValidator : AbstractValidator<AddProductVariantCommand>
    {
        public AddProductVariantValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be zero or greater.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleFor(x => x.OptionValueIds)
                .NotEmpty().WithMessage("At least one option value must be selected.")
                .Must(x => x != null && x.Count > 0).WithMessage("Option values are required.");
        }
    }

}
