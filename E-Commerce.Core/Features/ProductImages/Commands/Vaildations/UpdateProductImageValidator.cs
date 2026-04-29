using E_Commerce.Core.Features.ProductImages.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductImages.Commands.Vaildations
{
    public class UpdateProductImageValidator : AbstractValidator<UpdateProductImageCommand>
    {
        private static readonly string[] AllowedTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
        private const long MaxBytes = 5 * 1024 * 1024;

        public UpdateProductImageValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.ImageId).NotEmpty();

            When(x => x.File != null, () => {
                RuleFor(x => x.File!.Length).LessThanOrEqualTo(MaxBytes)
                    .WithMessage("File size must not exceed 5MB.");
                RuleFor(x => x.File!.ContentType)
                    .Must(t => AllowedTypes.Contains(t?.ToLower()))
                    .WithMessage("Only JPG, PNG, and WebP allowed.");
            });
        }
    }
}
