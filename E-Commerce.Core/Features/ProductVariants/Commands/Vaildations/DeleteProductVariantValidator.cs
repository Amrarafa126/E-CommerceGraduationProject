using E_Commerce.Core.Features.ProductVariants.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductVariants.Commands.Vaildations
{  
      public class DeleteProductVariantValidator : AbstractValidator<DeleteProductVariantCommand>
        {
            public DeleteProductVariantValidator()
            {
                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("ProductId is required.");

                RuleFor(x => x.VariantId)
                    .NotEmpty().WithMessage("VariantId is required.");
            }
        }
    }

