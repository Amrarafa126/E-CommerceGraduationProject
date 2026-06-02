using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.ProductVariants.Commands.Vaildations
{
    public class AddProductVariantValidator : AbstractValidator<AddProductVariantCommand>
    {
        public AddProductVariantValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("معرف المنتج مطلوب.");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("رمز SKU مطلوب.")
                .MaximumLength(50).WithMessage("رمز SKU لا يجب أن يتجاوز 50 حرف.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("السعر يجب أن يكون صفر أو أكبر.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("المخزون لا يمكن أن يكون سالباً.");

            RuleFor(x => x.OptionValueIds)
                .NotNull().WithMessage("قيم الخيارات مطلوبة.");
        }
    }
}
