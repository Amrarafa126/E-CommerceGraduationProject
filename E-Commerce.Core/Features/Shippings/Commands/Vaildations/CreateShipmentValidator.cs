using E_Commerce.Core.Features.Shipping.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings.Commands.Vaildations
{
    public class CreateShipmentValidator : AbstractValidator<CreateShipmentCommand>
    {
        private static readonly string[] ValidMethods = ["Standard", "Express", "Overnight", "Pickup"];

        public CreateShipmentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(300);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.State).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Method).NotEmpty()
                .Must(m => ValidMethods.Contains(m))
                .WithMessage($"Method must be one of: {string.Join(", ", ValidMethods)}");
            RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0);
        }
    }
}
