using MediatR;

namespace E_Commerce.Core.Features.Chats.Queries.Models
{
    public record GetConversationMessagesQuery(Guid ConversationId, int Page = 1, int PageSize = 30)
        : IRequest<ApiResponse<ConversationPageDto>>;
}
