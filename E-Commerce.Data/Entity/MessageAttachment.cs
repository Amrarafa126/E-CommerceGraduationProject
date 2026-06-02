using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class MessageAttachment : BaseEntity
    {
        public Guid MessageId { get; private set; }
        public Message Message { get; private set; } = null!;
        public string FileName { get; private set; } = string.Empty;
        public string FileUrl { get; private set; } = string.Empty;
        public AttachmentType Type { get; private set; }
        public long FileSize { get; private set; }
        public int? DurationSeconds { get; private set; }
        public string? MimeType { get; private set; }

        private MessageAttachment() { }

        public static MessageAttachment Create(
            Guid messageId,
            string fileName,
            string fileUrl,
            AttachmentType type,
            long fileSize,
            string? mimeType = null,
            int? durationSeconds = null)
        {
            return new MessageAttachment
            {
                MessageId = messageId,
                FileName = fileName,
                FileUrl = fileUrl,
                Type = type,
                FileSize = fileSize,
                MimeType = mimeType,
                DurationSeconds = durationSeconds
            };
        }
    }
}
