using MediatR;


namespace E_Commerce.Core.Features.Chats.Commands.Models
{
    public record MarkMessagesReadCommand(Guid ConversationId) : IRequest<ApiResponse<object>>;

}
