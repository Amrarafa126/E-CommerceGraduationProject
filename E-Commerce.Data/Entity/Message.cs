using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;

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
        public MessageType Type { get; private set; } = MessageType.Text;
        public bool IsDeleted { get; private set; }

        // Reply threading
        public Guid? ReplyToMessageId { get; private set; }
        public Message? ReplyToMessage { get; private set; }

        // Rich content
        public string? CardDataJson { get; private set; } // Product/Order/Quotation card serialized data

        // Navigation
        public ICollection<MessageAttachment> Attachments { get; private set; } = new List<MessageAttachment>();
        public ICollection<MessageReadReceipt> ReadReceipts { get; private set; } = new List<MessageReadReceipt>();

        private Message() { }

        public static Message Create(
            Guid conversationId,
            Guid senderId,
            string content,
            MessageType type = MessageType.Text,
            Guid? replyToMessageId = null,
            string? cardDataJson = null)
        {
            return new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                Type = type,
                ReplyToMessageId = replyToMessageId,
                CardDataJson = cardDataJson
            };
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }

        public void AddAttachment(MessageAttachment attachment)
        {
            Attachments.Add(attachment);
        }
    }
}
