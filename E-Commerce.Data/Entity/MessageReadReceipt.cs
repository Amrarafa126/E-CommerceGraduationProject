using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class MessageReadReceipt : BaseEntity
    {
        public Guid MessageId { get; private set; }
        public Message Message { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public DateTime ReadAt { get; private set; }

        private MessageReadReceipt() { }

        public static MessageReadReceipt Create(Guid messageId, Guid userId)
        {
            return new MessageReadReceipt
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            };
        }
    }
}
