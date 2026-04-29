using E_Commerce.Core.Features.ProductImages.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductImages.Commands.Vaildations
{
    public class UploadProductImageValidator : AbstractValidator<UploadProductImageCommand>
    {
        private static readonly string[] AllowedTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
        private const long MaxBytes = 5 * 1024 * 1024;

        public UploadProductImageValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.File).NotNull().WithMessage("File is required.");
            RuleFor(x => x.File.Length).LessThanOrEqualTo(MaxBytes)
                .When(x => x.File != null).WithMessage("File size must not exceed 5MB.");
            RuleFor(x => x.File.ContentType)
                .Must(t => AllowedTypes.Contains(t?.ToLower()))
                .When(x => x.File != null).WithMessage("Only JPG, PNG, and WebP allowed.");
        }
    }
}
