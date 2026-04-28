using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Chats.Oueries.Models
{
    public record GetConversationMessagesQuery(Guid ConversationId, int Page = 1, int PageSize = 30)
       : IRequest<ApiResponse<ConversationPageDto>>;
}
