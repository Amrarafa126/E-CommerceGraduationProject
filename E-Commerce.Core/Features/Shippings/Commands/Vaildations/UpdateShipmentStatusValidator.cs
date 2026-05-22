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
        public UpdateShipmentStatusValidator()
        {
            RuleFor(x => x.ShipmentId).NotEmpty();
            RuleFor(x => x.Status).InclusiveBetween(0, 6)
                .WithMessage("Status must be 0 (ReadyForPickup), 1 (InTransit), 2 (OutForDelivery), 3 (Delivered), 4 (Failed), or 5 (Returned).");
            RuleFor(x => x.CarrierName).NotEmpty()
                .When(x => x.Status == 1)
                .WithMessage("CarrierName required when marking as InTransit.");
            RuleFor(x => x.TrackingNumber).NotEmpty()
                .When(x => x.Status == 1)
                .WithMessage("TrackingNumber required when marking as InTransit.");
        }
    }

}
