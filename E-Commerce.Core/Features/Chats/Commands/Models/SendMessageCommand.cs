using MediatR;

namespace E_Commerce.Core.Features.Chats.Commands.Models
{
    public record SendMessageCommand(
        Guid ConversationId,
        string Content,
        int Type,
        List<AttachmentUploadDto>? Attachments = null,
        Guid? ReplyToMessageId = null,
        string? CardDataJson = null,
        string? IdempotencyKey = null)
        : IRequest<ApiResponse<MessageDto>>;
}
