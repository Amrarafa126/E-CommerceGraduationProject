using E_Commerce.Core.Features.Shippings.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Shippings.Commands.Vaildations
{

    public class UpdateShipmentStatusValidator : AbstractValidator<UpdateShipmentStatusCommand>
    {
        private static readonly string[] ValidStatuses =
            ["ReadyForPickup", "InTransit", "OutForDelivery", "Delivered", "Failed", "Returned"];

        public UpdateShipmentStatusValidator()
        {
            RuleFor(x => x.ShipmentId).NotEmpty();
            RuleFor(x => x.Status).NotEmpty()
                .Must(s => ValidStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");
            RuleFor(x => x.CarrierName).NotEmpty()
                .When(x => x.Status == "InTransit")
                .WithMessage("CarrierName required when marking as InTransit.");
            RuleFor(x => x.TrackingNumber).NotEmpty()
                .When(x => x.Status == "InTransit")
                .WithMessage("TrackingNumber required when marking as InTransit.");
        }
    }

}
