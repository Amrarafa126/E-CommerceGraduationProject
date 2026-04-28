using E_Commerce.Core.Features.Chats.Commands.Models;
using FluentValidation;


namespace E_Commerce.Core.Features.Chats.Commands.Vaildations
{
    public class StartConversationValidator : AbstractValidator<StartConversationCommand>
    {
        public StartConversationValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.InitialMessage).NotEmpty().MaximumLength(2000);
        }
    }
}
