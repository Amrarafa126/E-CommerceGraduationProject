using E_Commerce.Core.Features.Products.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Products.Commands.Vaildations
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المنتج مطلوب.")
                .MaximumLength(300).WithMessage("اسم المنتج لا يجب أن يتجاوز 300 حرف.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("وصف المنتج مطلوب.")
                .MaximumLength(5000).WithMessage("الوصف لا يجب أن يتجاوز 5000 حرف.");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("معرف الفئة مطلوب.");
            RuleFor(x => x.MinimumOrderQuantity).GreaterThan(0).WithMessage("أقل كمية للطلب يجب أن تكون أكبر من صفر.");
            RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage("السعر الأساسي يجب أن يكون صفر أو أكبر.");
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("المخزون يجب أن يكون صفر أو أكبر.");
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("العملة مطلوبة.")
                .Length(3).WithMessage("رمز العملة يجب أن يكون 3 أحرف.");
            RuleFor(x => x.Brand).MaximumLength(100).WithMessage("العلامة التجارية لا يجب أن تتجاوز 100 حرف.");
            RuleFor(x => x.ModelNumber).MaximumLength(100).WithMessage("رقم الموديل لا يجب أن يتجاوز 100 حرف.");
            RuleFor(x => x.OriginCountry).MaximumLength(100).WithMessage("بلد المنشأ لا يجب أن تتجاوز 100 حرف.");
            RuleFor(x => x.SupplyAbility).MaximumLength(200).WithMessage("القدرة الإنتاجية لا يجب أن تتجاوز 200 حرف.");
            RuleFor(x => x.TradeTerms).MaximumLength(200).WithMessage("شروط التجارة لا يجب أن تتجاوز 200 حرف.");
            RuleFor(x => x.PortOfLoading).MaximumLength(200).WithMessage("ميناء الشحن لا يجب أن يتجاوز 200 حرف.");
            RuleFor(x => x.PaymentTerms).MaximumLength(500).WithMessage("شروط الدفع لا يجب أن تتجاوز 500 حرف.");
            RuleFor(x => x.PackagingDetails).MaximumLength(2000).WithMessage("تفاصيل التغليف لا يجب أن تتجاوز 2000 حرف.");
            RuleFor(x => x.MetaTitle).MaximumLength(200).WithMessage("عنوان SEO لا يجب أن يتجاوز 200 حرف.");
            RuleFor(x => x.MetaDescription).MaximumLength(500).WithMessage("وصف SEO لا يجب أن يتجاوز 500 حرف.");
            RuleFor(x => x.Slug).MaximumLength(300).WithMessage("الرابط المختصر لا يجب أن يتجاوز 300 حرف.");
        }
    }
}
