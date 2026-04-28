using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Chats
{
    public record ConversationDto(Guid Id, Guid BuyerId, string BuyerName, string? BuyerEmail, Guid CompanyId, string CompanyName, string? CompanyLogoUrl, int UnreadCount, MessageDto? LastMessage, DateTime? LastMessageAt, DateTime CreatedAt);
    public record MessageDto(Guid Id, Guid ConversationId, Guid SenderId, string SenderName, string Content, string? AttachmentUrl, string Type, bool IsRead, DateTime? ReadAt, DateTime CreatedAt);
    public record SendMessageDto(string Content, string? AttachmentUrl = null, string Type = "Text");
    public record ConversationPageDto(ConversationDto Conversation, List<MessageDto> Messages, int TotalMessages, int Page, int PageSize);
}
