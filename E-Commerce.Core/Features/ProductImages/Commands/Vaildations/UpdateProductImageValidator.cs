using E_Commerce.Core.Features.ProductImages.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.ProductImages.Commands.Vaildations
{
    public class UpdateProductImageValidator : AbstractValidator<UpdateProductImageCommand>
    {
        private static readonly string[] AllowedTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
        private const long MaxBytes = 5 * 1024 * 1024;

        public UpdateProductImageValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("معرف المنتج مطلوب.");
            RuleFor(x => x.ImageId).NotEmpty().WithMessage("معرف الصورة مطلوب.");

            When(x => x.File != null, () => {
                RuleFor(x => x.File!.Length)
                    .LessThanOrEqualTo(MaxBytes)
                    .WithMessage("حجم الملف لا يجب أن يتجاوز 5 ميجابايت.");
                RuleFor(x => x.File!.ContentType)
                    .Must(t => AllowedTypes.Contains(t?.ToLower()))
                    .WithMessage("يُسمح فقط بصيغ JPG و PNG و WebP.");
            });
        }
    }
}
