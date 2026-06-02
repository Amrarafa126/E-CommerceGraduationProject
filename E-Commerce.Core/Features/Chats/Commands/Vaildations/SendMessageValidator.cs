using E_Commerce.Core.Features.Chats.Commands.Models;
using FluentValidation;

namespace E_Commerce.Core.Features.Chats.Commands.Vaildations
{
    public class SendMessageValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.ConversationId).NotEmpty().WithMessage("معرف المحادثة مطلوب.");
            RuleFor(x => x.Content)
                .MaximumLength(2000).WithMessage("محتوى الرسالة لا يجب أن يتجاوز 2000 حرف.");
            RuleFor(x => x)
                .Must(cmd => !string.IsNullOrWhiteSpace(cmd.Content) || (cmd.Attachments?.Count > 0))
                .WithMessage("الرسالة يجب أن تحتوي على نص أو مرفق واحد على الأقل.");
        }
    }
}
