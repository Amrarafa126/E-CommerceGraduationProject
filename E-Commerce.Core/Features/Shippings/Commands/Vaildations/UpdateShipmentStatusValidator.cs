using E_Commerce.Core.Features.Shippings.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Shippings.Commands.Vaildations
{
    public class UpdateShipmentStatusValidator : AbstractValidator<UpdateShipmentStatusCommand>
    {
        public UpdateShipmentStatusValidator()
        {
            RuleFor(x => x.ShipmentId).NotEmpty().WithMessage("معرف الشحنة مطلوب.");
            RuleFor(x => x.Status)
                .InclusiveBetween(0, 6)
                .WithMessage("الحالة يجب أن تكون 0 (جاهز للاستلام)، 1 (في الطريق)، 2 (خارج للتوصيل)، 3 (تم التوصيل)، 4 (فشل)، 5 (مرتجع)، أو 6 (ملغى).");
            RuleFor(x => x.CarrierName)
                .NotEmpty().WithMessage("اسم شركة الشحن مطلوب عندما تكون في الطريق.")
                .When(x => x.Status == 1);
            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("رقم التتبع مطلوب عندما تكون في الطريق.")
                .When(x => x.Status == 1);
        }
    }
}
