using E_Commerce.Core.Features.Payment.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Payment.Commands.Validation
{
    public class RequestPayoutValidator : AbstractValidator<RequestPayoutCommand>
    {
        public RequestPayoutValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("المبلغ يجب أن يكون أكبر من صفر.");
        }
    }
}
