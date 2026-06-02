using E_Commerce.Core.Wrappers;
using MediatR;

namespace E_Commerce.Core.Features.Chats.Queries.Models
{
    public record GetConversationsQuery(int Page = 1, int PageSize = 20)
        : IRequest<ApiResponse<PaginatedResult<ConversationDto>>>;
}
