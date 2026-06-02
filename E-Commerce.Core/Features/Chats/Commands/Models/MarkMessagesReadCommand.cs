using MediatR;

namespace E_Commerce.Core.Features.Chats.Commands.Models
{
    public record MarkMessagesReadCommand(
        Guid ConversationId,
        List<Guid> MessageIds)
        : IRequest<ApiResponse<object>>;
}
