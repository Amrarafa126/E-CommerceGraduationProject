using E_Commerce.Core.Features.Chats.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Chats.Commands.Vaildations
{
    public class StartConversationValidator : AbstractValidator<StartConversationCommand>
    {
        public StartConversationValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty().WithMessage("معرف الشركة مطلوب.");
            RuleFor(x => x.InitialMessage)
                .NotEmpty().WithMessage("الرسالة الأولى مطلوبة.")
                .MaximumLength(2000).WithMessage("الرسالة الأولى لا يجب أن تتجاوز 2000 حرف.");
        }
    }
}
