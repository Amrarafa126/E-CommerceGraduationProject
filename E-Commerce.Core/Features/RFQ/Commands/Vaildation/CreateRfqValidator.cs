using E_Commerce.Core.Features.RFQ.Commands.Models;
using FluentValidation;


namespace E_Commerce.Core.Features.RFQ.Commands.Vaildation
{
    public class CreateRfqValidator : AbstractValidator<CreateRfqCommand>
    {
        public CreateRfqValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(3000);
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.SellerCompanyId).NotEmpty();
            RuleFor(x => x.DeadlineDate).GreaterThan(DateTime.UtcNow).When(x => x.DeadlineDate.HasValue);
        }
    }
}
