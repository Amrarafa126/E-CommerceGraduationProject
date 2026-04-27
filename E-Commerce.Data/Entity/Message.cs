using E_Commerce.Data.Identity;


namespace E_Commerce.Data.Entity
{
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; private set; }
        public Conversation Conversation { get; private set; } = null!;
        public Guid SenderId { get; private set; }
        public User Sender { get; private set; } = null!;
        public string Content { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public string? AttachmentUrl { get; private set; }
        public MessageType Type { get; private set; } = MessageType.Text;

        private Message() { }

        public static Message Create(Guid conversationId, Guid senderId, string content,
            string? attachmentUrl = null, MessageType type = MessageType.Text) => new()
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                AttachmentUrl = attachmentUrl,
                Type = type
            };

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
    }

    public enum MessageType { Text, Image, File, ProductLink }
}
