using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class Conversation : BaseEntity
    {
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;
        public string? Subject { get; private set; }
        public bool IsBuyerArchived { get; private set; }
        public bool IsCompanyArchived { get; private set; }
        public DateTime? LastMessageAt { get; private set; }

        // Rich context links
        public Guid? RelatedProductId { get; private set; }
        public Guid? RelatedOrderId { get; private set; }
        public Guid? RelatedRfqId { get; private set; }

        public ICollection<Message> Messages { get; private set; } = new List<Message>();

        private Conversation() { }

        public static Conversation Create(Guid buyerId, Guid companyId, string? subject = null) => new()
        {
            BuyerId = buyerId,
            CompanyId = companyId,
            Subject = subject,
            LastMessageAt = DateTime.UtcNow
        };

        public void TouchLastMessage()
        {
            LastMessageAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void ArchiveForBuyer() { IsBuyerArchived = true; MarkAsUpdated(); }
        public void ArchiveForCompany() { IsCompanyArchived = true; MarkAsUpdated(); }
        public void UnarchiveForBuyer() { IsBuyerArchived = false; MarkAsUpdated(); }
        public void UnarchiveForCompany() { IsCompanyArchived = false; MarkAsUpdated(); }

        public void SetRelatedProduct(Guid productId)
        {
            RelatedProductId = productId;
            MarkAsUpdated();
        }

        public void SetRelatedOrder(Guid orderId)
        {
            RelatedOrderId = orderId;
            MarkAsUpdated();
        }

        public void SetRelatedRfq(Guid rfqId)
        {
            RelatedRfqId = rfqId;
            MarkAsUpdated();
        }
    }
}
