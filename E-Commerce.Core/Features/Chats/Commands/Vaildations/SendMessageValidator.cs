using E_Commerce.Core.Features.Chats.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Chats.Commands.Vaildations
{
    public class SendMessageValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.ConversationId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
        }
    }
}
