using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.ProductVariants.Commands.Vaildations
{
    public class DeleteProductVariantValidator : AbstractValidator<DeleteProductVariantCommand>
    {
        public DeleteProductVariantValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("معرف المنتج مطلوب.");

            RuleFor(x => x.VariantId)
                .NotEmpty().WithMessage("معرف المتغير مطلوب.");
        }
    }
}
