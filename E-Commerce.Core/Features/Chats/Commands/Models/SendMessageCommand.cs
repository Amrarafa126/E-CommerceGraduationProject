using MediatR;


namespace E_Commerce.Core.Features.Chats.Commands.Models
{
    public record SendMessageCommand(Guid ConversationId, string Content,
       string? AttachmentUrl = null, int Type = 0)
       : IRequest<ApiResponse<MessageDto>>;
}
