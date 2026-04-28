using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Chats.Commands.Models
{
    public record StartConversationCommand(Guid CompanyId, string InitialMessage)
       : IRequest<ApiResponse<ConversationDto>>;
}
