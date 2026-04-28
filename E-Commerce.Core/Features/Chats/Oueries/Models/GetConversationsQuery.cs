using E_Commerce.Core.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Chats.Oueries.Models
{
    public record GetConversationsQuery(int Page = 1, int PageSize = 20)
      : IRequest<ApiResponse<PaginatedResult<ConversationDto>>>;
}
