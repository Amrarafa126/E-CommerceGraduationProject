namespace E_Commerce.Core.Features.Chats
{
    // ── Conversation DTOs ───────────────────────────────────────────
    public class ConversationDto
    {
        public Guid Id { get; init; }
        public Guid BuyerId { get; init; }
        public string BuyerName { get; init; } = string.Empty;
        public string? BuyerEmail { get; init; }
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = string.Empty;
        public string? CompanyLogoUrl { get; init; }
        public string? Subject { get; init; }
        public int UnreadCount { get; init; }
        public MessageDto? LastMessage { get; init; }
        public DateTime? LastMessageAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public Guid? RelatedProductId { get; init; }
        public Guid? RelatedOrderId { get; init; }
        public Guid? RelatedRfqId { get; init; }
    }

    // ── Message DTOs ────────────────────────────────────────────────
    public class MessageDto
    {
        public Guid Id { get; init; }
        public Guid ConversationId { get; init; }
        public Guid SenderId { get; init; }
        public string SenderName { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public int Type { get; init; }
        public List<AttachmentDto> Attachments { get; init; } = new();
        public Guid? ReplyToMessageId { get; init; }
        public MessageDto? ReplyToMessage { get; init; }
        public string? CardDataJson { get; init; }
        public List<ReadReceiptDto> ReadBy { get; init; } = new();
        public DateTime CreatedAt { get; init; }
    }

    public class AttachmentDto
    {
        public Guid Id { get; init; }
        public string FileName { get; init; } = string.Empty;
        public string FileUrl { get; init; } = string.Empty;
        public int Type { get; init; }
        public long FileSize { get; init; }
        public int? DurationSeconds { get; init; }
        public string? MimeType { get; init; }
    }

    public class ReadReceiptDto
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public DateTime ReadAt { get; init; }
    }

    // ── Request/Response DTOs ───────────────────────────────────────
    public class ConversationPageDto
    {
        public ConversationDto Conversation { get; init; } = null!;
        public List<MessageDto> Messages { get; init; } = new();
        public int TotalMessages { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }

    public class SendMessageRequest
    {
        public string Content { get; init; } = string.Empty;
        public int Type { get; init; }
        public List<AttachmentUploadDto>? Attachments { get; init; }
        public Guid? ReplyToMessageId { get; init; }
        public string? CardDataJson { get; init; }
    }

    public class AttachmentUploadDto
    {
        public string FileName { get; init; } = string.Empty;
        public string FileUrl { get; init; } = string.Empty;
        public int Type { get; init; }
        public long FileSize { get; init; }
        public int? DurationSeconds { get; init; }
        public string? MimeType { get; init; }
    }

    public class MarkReadRequest
    {
        public List<Guid> MessageIds { get; init; } = new();
    }

    public class StartConversationRequest
    {
        public Guid CompanyId { get; init; }
        public string InitialMessage { get; init; } = string.Empty;
        public string? Subject { get; init; }
        public Guid? RelatedProductId { get; init; }
        public Guid? RelatedOrderId { get; init; }
        public Guid? RelatedRfqId { get; init; }
    }

    // ── SignalR DTOs ────────────────────────────────────────────────
    public class TypingIndicatorDto
    {
        public Guid ConversationId { get; init; }
        public Guid UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public bool IsTyping { get; init; }
    }

    public class MessageReceivedDto
    {
        public Guid ConversationId { get; init; }
        public MessageDto Message { get; init; } = null!;
    }

    public class ReadReceiptBroadcastDto
    {
        public Guid ConversationId { get; init; }
        public List<Guid> MessageIds { get; init; } = new();
        public Guid ReaderId { get; init; }
        public string ReaderName { get; init; } = string.Empty;
        public DateTime ReadAt { get; init; }
    }
}
